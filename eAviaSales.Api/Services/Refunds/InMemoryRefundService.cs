namespace eAviaSales.Api.Services.Refunds;

public sealed class InMemoryRefundService : IRefundService
{
    private readonly object _sync = new();
    private readonly Dictionary<string, string> _statusByRefundId = new(StringComparer.OrdinalIgnoreCase);

    public void UpsertStatusFromWebhook(string refundId, string status)
    {
        lock (_sync)
        {
            _statusByRefundId[refundId] = status.Trim();
        }
    }

    public string? GetStatus(string refundId)
    {
        lock (_sync)
        {
            return _statusByRefundId.TryGetValue(refundId, out var s) ? s : null;
        }
    }
}
