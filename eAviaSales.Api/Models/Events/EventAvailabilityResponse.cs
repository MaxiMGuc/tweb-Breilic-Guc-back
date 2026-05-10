namespace eAviaSales.Api.Models.Events;

public sealed class EventAvailabilityResponse
{
    public int EventId { get; init; }
    public int TotalSeats { get; init; }
    public int AvailableSeats { get; init; }
    public int HeldSeats { get; init; }
    public int SoldSeats { get; init; }
    public string AvailabilityStatus { get; init; } = string.Empty;
}

