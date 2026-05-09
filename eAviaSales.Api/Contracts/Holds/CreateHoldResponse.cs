namespace eAviaSales.Api.Contracts.Holds;

public sealed class CreateHoldResponse
{
    public string HoldId { get; init; } = string.Empty;
    public int EventId { get; init; }
    public IReadOnlyList<string> SeatNumbers { get; init; } = [];
    public DateTime ExpiresAtUtc { get; init; }
}
