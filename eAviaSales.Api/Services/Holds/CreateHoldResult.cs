namespace eAviaSales.Api.Services.Holds;

public sealed class CreateHoldResult
{
    public bool Success { get; init; }
    public HoldTicket? Hold { get; init; }
    public string? ConflictSeat { get; init; }
}
