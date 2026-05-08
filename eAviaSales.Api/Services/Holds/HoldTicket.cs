namespace eAviaSales.Api.Services.Holds;

public sealed class HoldTicket
{
    public string HoldId { get; init; } = string.Empty;
    public int EventId { get; init; }
    public IReadOnlyList<string> SeatNumbers { get; init; } = [];
    public DateTime ExpiresAtUtc { get; init; }
}
