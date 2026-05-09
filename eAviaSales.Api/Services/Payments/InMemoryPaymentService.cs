using eAviaSales.Api.Services.Orders;

namespace eAviaSales.Api.Services.Payments;

public sealed class InMemoryPaymentService : IPaymentService
{
    public const string Pending = "pending";
    public const string Succeeded = "succeeded";
    public const string Failed = "failed";

    private readonly IOrderService _orderService;
    private readonly object _sync = new();
    private readonly Dictionary<string, PaymentRecord> _payments = new(StringComparer.OrdinalIgnoreCase);

    public InMemoryPaymentService(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public OrderServiceResult<PaymentRecord> CreateIntent(string orderId)
    {
        var order = _orderService.GetOrder(orderId);
        if (order is null)
        {
            return OrderServiceResult<PaymentRecord>.Fail("order_not_found", "Order was not found.");
        }

        if (order.Status != InMemoryOrderService.PendingPayment)
        {
            return OrderServiceResult<PaymentRecord>.Fail(
                "invalid_order_state",
                "Payment intents can only be created for orders in pending_payment.");
        }

        var paymentId = Guid.NewGuid().ToString("N");
        var record = new PaymentRecord
        {
            PaymentId = paymentId,
            OrderId = orderId,
            Amount = order.TotalAmount,
            CurrencyCode = order.CurrencyCode,
            Status = Pending
        };

        lock (_sync)
        {
            _payments[paymentId] = record;
        }

        return OrderServiceResult<PaymentRecord>.Ok(record);
    }

    public OrderServiceResult<PaymentRecord> Confirm(string paymentId)
    {
        return TransitionToSucceeded(paymentId);
    }

    public PaymentRecord? Get(string paymentId)
    {
        lock (_sync)
        {
            return _payments.TryGetValue(paymentId, out var r) ? Clone(r) : null;
        }
    }

    public OrderServiceResult<PaymentRecord> ApplyExternalStatus(string paymentId, string status)
    {
        var normalized = status.Trim().ToLowerInvariant();
        if (normalized is Succeeded or "completed" or "paid")
        {
            return TransitionToSucceeded(paymentId);
        }

        if (normalized is Failed or "cancelled")
        {
            lock (_sync)
            {
                if (!_payments.TryGetValue(paymentId, out var record))
                {
                    return OrderServiceResult<PaymentRecord>.Fail("payment_not_found", "Payment was not found.");
                }

                if (record.Status == Succeeded)
                {
                    return OrderServiceResult<PaymentRecord>.Ok(Clone(record));
                }

                record.Status = Failed;
                return OrderServiceResult<PaymentRecord>.Ok(Clone(record));
            }
        }

        lock (_sync)
        {
            if (!_payments.TryGetValue(paymentId, out var record))
            {
                return OrderServiceResult<PaymentRecord>.Fail("payment_not_found", "Payment was not found.");
            }

            return OrderServiceResult<PaymentRecord>.Ok(Clone(record));
        }
    }

    private OrderServiceResult<PaymentRecord> TransitionToSucceeded(string paymentId)
    {
        lock (_sync)
        {
            if (!_payments.TryGetValue(paymentId, out var record))
            {
                return OrderServiceResult<PaymentRecord>.Fail(
                    "payment_not_found",
                    "Payment was not found.");
            }

            if (record.Status == Succeeded)
            {
                return OrderServiceResult<PaymentRecord>.Ok(Clone(record));
            }

            record.Status = Succeeded;
            if (!_orderService.MarkOrderPaid(record.OrderId))
            {
                record.Status = Failed;
                return OrderServiceResult<PaymentRecord>.Fail(
                    "order_payment_failed",
                    "Could not mark order as paid.");
            }

            return OrderServiceResult<PaymentRecord>.Ok(Clone(record));
        }
    }

    private static PaymentRecord Clone(PaymentRecord r)
    {
        return new PaymentRecord
        {
            PaymentId = r.PaymentId,
            OrderId = r.OrderId,
            Amount = r.Amount,
            CurrencyCode = r.CurrencyCode,
            Status = r.Status
        };
    }
}
