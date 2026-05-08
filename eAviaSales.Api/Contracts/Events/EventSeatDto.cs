namespace eAviaSales.Api.Contracts.Events;

public sealed class EventSeatDto
{
    public string SeatNumber { get; init; } = string.Empty;
    public string CabinClass { get; init; } = "Economy";
    public string Status { get; init; } = string.Empty;
}
