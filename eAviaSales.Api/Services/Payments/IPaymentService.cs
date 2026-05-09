using eAviaSales.Api.Services;

namespace eAviaSales.Api.Services.Payments;

public interface IPaymentService
{
    ServiceResult<PaymentRecord> CreateIntent(string orderId);

    ServiceResult<PaymentRecord> Confirm(string paymentId);

    PaymentRecord? Get(string paymentId);

    ServiceResult<PaymentRecord> ApplyExternalStatus(string paymentId, string status);
}
