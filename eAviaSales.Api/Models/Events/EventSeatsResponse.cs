namespace eAviaSales.Api.Models.Events;

public sealed class EventSeatsResponse
{
    public int EventId { get; init; }
    public IReadOnlyList<EventSeatDto> Seats { get; init; } = [];
}

