using eAviaSales.Api.Services;
using eAviaSales.Api.Services.Cart;
using eAviaSales.Api.Services.Holds;
using eAviaSales.Api.Services.Orders;
using eAviaSales.Api.Services.Payments;
using eAviaSales.Api.Services.Refunds;

namespace eAviaSales.Api.Services.Ticketing;

/// <summary>One in-memory store for cart, holds, orders, payments, and refund webhook state.</summary>
public sealed class TicketingMemoryStore : ICartService, IHoldService, IOrderService, IPaymentService, IRefundService
{
    public const string OrderPendingPayment = "pending_payment";
    public const string OrderPaid = "paid";
    public const string OrderCancelled = "cancelled";

    private const string PaymentPending = "pending";
    private const string PaymentSucceeded = "succeeded";
    private const string PaymentFailed = "failed";

    private readonly object _lock = new();

    private readonly Dictionary<string, Dictionary<string, CartItem>> _carts = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, HoldTicket> _holds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _seatReservations = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, CheckoutRecord> _checkouts = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, OrderRecord> _orders = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, PaymentRecord> _payments = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _refundStatuses = new(StringComparer.OrdinalIgnoreCase);

    #region Cart (ICartService)

    CartSnapshot ICartService.Get(string cartId) => GetCartSnapshot(cartId);

    private CartSnapshot GetCartSnapshot(string cartId)
    {
        lock (_lock)
        {
            if (!_carts.TryGetValue(cartId, out var cart))
            {
                return new CartSnapshot { CartId = cartId };
            }

            return new CartSnapshot { CartId = cartId, Items = cart.Values.Select(CloneCartItem).ToList() };
        }
    }

    public CartSnapshot AddOrUpdateItem(
        string cartId,
        int eventId,
        string eventCode,
        int quantity,
        decimal unitPrice,
        string currencyCode)
    {
        lock (_lock)
        {
            if (!_carts.TryGetValue(cartId, out var cart))
            {
                cart = new Dictionary<string, CartItem>(StringComparer.OrdinalIgnoreCase);
                _carts[cartId] = cart;
            }

            var existing = cart.Values.FirstOrDefault(item => item.EventId == eventId);
            if (existing is not null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                var item = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString("N"),
                    EventId = eventId,
                    EventCode = eventCode,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    CurrencyCode = currencyCode
                };
                cart[item.ItemId] = item;
            }

            return new CartSnapshot { CartId = cartId, Items = cart.Values.Select(CloneCartItem).ToList() };
        }
    }

    public bool RemoveItem(string cartId, string itemId)
    {
        lock (_lock)
        {
            return _carts.TryGetValue(cartId, out var cart) && cart.Remove(itemId);
        }
    }

    public void Clear(string cartId)
    {
        lock (_lock)
        {
            _carts.Remove(cartId);
        }
    }

    private static CartItem CloneCartItem(CartItem source) =>
        new()
        {
            ItemId = source.ItemId,
            EventId = source.EventId,
            EventCode = source.EventCode,
            Quantity = source.Quantity,
            UnitPrice = source.UnitPrice,
            CurrencyCode = source.CurrencyCode
        };

    #endregion

    #region Holds (IHoldService)

    public CreateHoldResult Create(int eventId, IReadOnlyList<string> seatNumbers, int holdMinutes)
    {
        var normalizedSeats = seatNumbers
            .Where(static seat => !string.IsNullOrWhiteSpace(seat))
            .Select(static seat => seat.Trim().ToUpperInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedSeats.Count == 0)
        {
            return new CreateHoldResult { Success = false };
        }

        lock (_lock)
        {
            RemoveExpiredHoldsUnsafe();

            foreach (var seat in normalizedSeats)
            {
                var key = SeatKey(eventId, seat);
                if (_seatReservations.ContainsKey(key))
                {
                    return new CreateHoldResult { Success = false, ConflictSeat = seat };
                }
            }

            var holdId = Guid.NewGuid().ToString("N");
            var hold = new HoldTicket
            {
                HoldId = holdId,
                EventId = eventId,
                SeatNumbers = normalizedSeats,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(holdMinutes)
            };

            _holds[holdId] = hold;
            foreach (var seat in normalizedSeats)
            {
                _seatReservations[SeatKey(eventId, seat)] = holdId;
            }

            return new CreateHoldResult { Success = true, Hold = hold };
        }
    }

    public bool Delete(int eventId, string holdId)
    {
        lock (_lock)
        {
            RemoveExpiredHoldsUnsafe();

            if (!_holds.TryGetValue(holdId, out var hold) || hold.EventId != eventId)
            {
                return false;
            }

            _holds.Remove(holdId);
            foreach (var seat in hold.SeatNumbers)
            {
                _seatReservations.Remove(SeatKey(eventId, seat));
            }

            return true;
        }
    }

    private void RemoveExpiredHoldsUnsafe()
    {
        var now = DateTime.UtcNow;
        foreach (var holdId in _holds.Values.Where(h => h.ExpiresAtUtc <= now).Select(h => h.HoldId).ToList())
        {
            if (_holds.Remove(holdId, out var removed))
            {
                foreach (var seat in removed.SeatNumbers)
                {
                    _seatReservations.Remove(SeatKey(removed.EventId, seat));
                }
            }
        }
    }

    private static string SeatKey(int eventId, string seatNumber) => $"{eventId}:{seatNumber}";

    #endregion

    #region Orders (IOrderService)

    public ServiceResult<CheckoutRecord> CreateCheckout(string customerKey)
    {
        var cart = GetCartSnapshot(customerKey);
        if (cart.Items.Count == 0)
        {
            return ServiceResult<CheckoutRecord>.Fail("cart_empty", "Cart has no items.");
        }

        var lines = cart.Items
            .Select(item => new OrderLineRecord
            {
                EventId = item.EventId,
                EventCode = item.EventCode,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            })
            .ToList();

        var currency = cart.Items[0].CurrencyCode;
        var total = cart.Items.Sum(static i => i.Quantity * i.UnitPrice);

        var checkout = new CheckoutRecord
        {
            CheckoutId = Guid.NewGuid().ToString("N"),
            CustomerKey = customerKey,
            Lines = lines,
            TotalAmount = total,
            CurrencyCode = currency,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15)
        };

        lock (_lock)
        {
            _checkouts[checkout.CheckoutId] = checkout;
        }

        return ServiceResult<CheckoutRecord>.Ok(checkout);
    }

    public ServiceResult<OrderRecord> PlaceOrder(string customerKey, string checkoutId)
    {
        CheckoutRecord checkout;
        lock (_lock)
        {
            if (!_checkouts.TryGetValue(checkoutId, out var c))
            {
                return ServiceResult<OrderRecord>.Fail("checkout_not_found", "Checkout not found.");
            }

            if (c.CustomerKey != customerKey)
            {
                return ServiceResult<OrderRecord>.Fail("checkout_forbidden", "Checkout does not belong to this session.");
            }

            if (c.ExpiresAtUtc <= DateTime.UtcNow)
            {
                _checkouts.Remove(checkoutId);
                return ServiceResult<OrderRecord>.Fail("checkout_expired", "Checkout has expired.");
            }

            checkout = c;
            _checkouts.Remove(checkoutId);
        }

        var order = new OrderRecord
        {
            OrderId = Guid.NewGuid().ToString("N"),
            CustomerKey = customerKey,
            Status = OrderPendingPayment,
            Lines = checkout.Lines.Select(static l => new OrderLineRecord
            {
                EventId = l.EventId,
                EventCode = l.EventCode,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice
            }).ToList(),
            TotalAmount = checkout.TotalAmount,
            CurrencyCode = checkout.CurrencyCode,
            CreatedAtUtc = DateTime.UtcNow
        };

        Clear(customerKey);

        lock (_lock)
        {
            _orders[order.OrderId] = order;
        }

        return ServiceResult<OrderRecord>.Ok(order);
    }

    public OrderRecord? GetOrder(string orderId)
    {
        lock (_lock)
        {
            return _orders.TryGetValue(orderId, out var o) ? CloneOrder(o) : null;
        }
    }

    public IReadOnlyList<OrderRecord> ListOrdersForCustomer(string customerKey)
    {
        lock (_lock)
        {
            return _orders.Values
                .Where(o => o.CustomerKey == customerKey)
                .OrderByDescending(static o => o.CreatedAtUtc)
                .Select(CloneOrder)
                .ToList();
        }
    }

    public ServiceResult<object> CancelOrder(string customerKey, string orderId)
    {
        lock (_lock)
        {
            if (!_orders.TryGetValue(orderId, out var order))
            {
                return ServiceResult<object>.Fail("order_not_found", "Order was not found.");
            }

            if (order.CustomerKey != customerKey)
            {
                return ServiceResult<object>.Fail("order_forbidden", "Order does not belong to this session.");
            }

            if (order.Status != OrderPendingPayment)
            {
                return ServiceResult<object>.Fail(
                    "invalid_order_state",
                    $"Order cannot be cancelled in state {order.Status}.");
            }

            order.Status = OrderCancelled;
        }

        return ServiceResult<object>.Ok(null!);
    }

    public ServiceResult<IReadOnlyList<OrderTicketRecord>> IssueTickets(string orderId)
    {
        lock (_lock)
        {
            if (!_orders.TryGetValue(orderId, out var order))
            {
                return ServiceResult<IReadOnlyList<OrderTicketRecord>>.Fail("order_not_found", "Order was not found.");
            }

            if (order.Status != OrderPaid)
            {
                return ServiceResult<IReadOnlyList<OrderTicketRecord>>.Fail(
                    "invalid_order_state",
                    "Tickets can only be issued for paid orders.");
            }

            if (order.Tickets.Count > 0)
            {
                return ServiceResult<IReadOnlyList<OrderTicketRecord>>.Ok(order.Tickets.ToList());
            }

            foreach (var line in order.Lines)
            {
                for (var i = 0; i < line.Quantity; i++)
                {
                    var ticketId = Guid.NewGuid().ToString("N");
                    var payload = $"{ticketId}:{line.EventId}:{order.OrderId}";
                    order.Tickets.Add(new OrderTicketRecord
                    {
                        TicketId = ticketId,
                        EventId = line.EventId,
                        EventCode = line.EventCode,
                        QrPayload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payload))
                    });
                }
            }

            return ServiceResult<IReadOnlyList<OrderTicketRecord>>.Ok(order.Tickets.ToList());
        }
    }

    public IReadOnlyList<OrderTicketRecord> GetTickets(string orderId)
    {
        lock (_lock)
        {
            if (!_orders.TryGetValue(orderId, out var order))
            {
                return [];
            }

            return order.Tickets.Select(static t => new OrderTicketRecord
            {
                TicketId = t.TicketId,
                EventId = t.EventId,
                EventCode = t.EventCode,
                QrPayload = t.QrPayload
            }).ToList();
        }
    }

    public bool MarkOrderPaid(string orderId)
    {
        lock (_lock)
        {
            return MarkOrderPaidUnsafe(orderId);
        }
    }

    /// <summary>Caller must hold <see cref="_lock"/>.</summary>
    private bool MarkOrderPaidUnsafe(string orderId)
    {
        if (!_orders.TryGetValue(orderId, out var order))
        {
            return false;
        }

        if (order.Status == OrderCancelled)
        {
            return false;
        }

        if (order.Status == OrderPaid)
        {
            return true;
        }

        order.Status = OrderPaid;
        return true;
    }

    private static OrderRecord CloneOrder(OrderRecord source) =>
        new()
        {
            OrderId = source.OrderId,
            CustomerKey = source.CustomerKey,
            Status = source.Status,
            Lines = source.Lines.Select(static l => new OrderLineRecord
            {
                EventId = l.EventId,
                EventCode = l.EventCode,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice
            }).ToList(),
            TotalAmount = source.TotalAmount,
            CurrencyCode = source.CurrencyCode,
            CreatedAtUtc = source.CreatedAtUtc,
            Tickets = source.Tickets.Select(static t => new OrderTicketRecord
            {
                TicketId = t.TicketId,
                EventId = t.EventId,
                EventCode = t.EventCode,
                QrPayload = t.QrPayload
            }).ToList()
        };

    #endregion

    #region Payments (IPaymentService)

    public ServiceResult<PaymentRecord> CreateIntent(string orderId)
    {
        var order = GetOrder(orderId);
        if (order is null)
        {
            return ServiceResult<PaymentRecord>.Fail("order_not_found", "Order was not found.");
        }

        if (order.Status != OrderPendingPayment)
        {
            return ServiceResult<PaymentRecord>.Fail(
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
            Status = PaymentPending
        };

        lock (_lock)
        {
            _payments[paymentId] = record;
        }

        return ServiceResult<PaymentRecord>.Ok(record);
    }

    public ServiceResult<PaymentRecord> Confirm(string paymentId) => TransitionPaymentSucceeded(paymentId);

    PaymentRecord? IPaymentService.Get(string paymentId) => GetPaymentRecord(paymentId);

    private PaymentRecord? GetPaymentRecord(string paymentId)
    {
        lock (_lock)
        {
            return _payments.TryGetValue(paymentId, out var r) ? ClonePayment(r) : null;
        }
    }

    public ServiceResult<PaymentRecord> ApplyExternalStatus(string paymentId, string status)
    {
        var normalized = status.Trim().ToLowerInvariant();
        if (normalized is PaymentSucceeded or "completed" or "paid")
        {
            return TransitionPaymentSucceeded(paymentId);
        }

        if (normalized is PaymentFailed or "cancelled")
        {
            lock (_lock)
            {
                if (!_payments.TryGetValue(paymentId, out var record))
                {
                    return ServiceResult<PaymentRecord>.Fail("payment_not_found", "Payment was not found.");
                }

                if (record.Status == PaymentSucceeded)
                {
                    return ServiceResult<PaymentRecord>.Ok(ClonePayment(record));
                }

                record.Status = PaymentFailed;
                return ServiceResult<PaymentRecord>.Ok(ClonePayment(record));
            }
        }

        lock (_lock)
        {
            if (!_payments.TryGetValue(paymentId, out var record))
            {
                return ServiceResult<PaymentRecord>.Fail("payment_not_found", "Payment was not found.");
            }

            return ServiceResult<PaymentRecord>.Ok(ClonePayment(record));
        }
    }

    private ServiceResult<PaymentRecord> TransitionPaymentSucceeded(string paymentId)
    {
        lock (_lock)
        {
            if (!_payments.TryGetValue(paymentId, out var record))
            {
                return ServiceResult<PaymentRecord>.Fail("payment_not_found", "Payment was not found.");
            }

            if (record.Status == PaymentSucceeded)
            {
                return ServiceResult<PaymentRecord>.Ok(ClonePayment(record));
            }

            record.Status = PaymentSucceeded;
            if (!MarkOrderPaidUnsafe(record.OrderId))
            {
                record.Status = PaymentFailed;
                return ServiceResult<PaymentRecord>.Fail("order_payment_failed", "Could not mark order as paid.");
            }

            return ServiceResult<PaymentRecord>.Ok(ClonePayment(record));
        }
    }

    private static PaymentRecord ClonePayment(PaymentRecord r) =>
        new()
        {
            PaymentId = r.PaymentId,
            OrderId = r.OrderId,
            Amount = r.Amount,
            CurrencyCode = r.CurrencyCode,
            Status = r.Status
        };

    #endregion

    #region Refunds (IRefundService)

    public void UpsertStatusFromWebhook(string refundId, string status)
    {
        lock (_lock)
        {
            _refundStatuses[refundId] = status.Trim();
        }
    }

    public string? GetStatus(string refundId)
    {
        lock (_lock)
        {
            return _refundStatuses.TryGetValue(refundId, out var s) ? s : null;
        }
    }

    #endregion
}
