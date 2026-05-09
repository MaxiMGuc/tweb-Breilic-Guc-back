using eAviaSales.Api.Services.Orders;

namespace eAviaSales.Api.Services.Payments;

public interface IPaymentService
{
    OrderServiceResult<PaymentRecord> CreateIntent(string orderId);

    OrderServiceResult<PaymentRecord> Confirm(string paymentId);

    PaymentRecord? Get(string paymentId);

    /// <summary>Apply external PSP status update (Idempotent).</summary>
    OrderServiceResult<PaymentRecord> ApplyExternalStatus(string paymentId, string status);
}
