namespace eAviaSales.Api.Services.Refunds;

public interface IRefundService
{
    void UpsertStatusFromWebhook(string refundId, string status);
    string? GetStatus(string refundId);
}
