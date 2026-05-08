namespace eAviaSales.Api.Contracts.Events;

public sealed class EventSeatsResponse
{
    public int EventId { get; init; }
    public IReadOnlyList<EventSeatDto> Seats { get; init; } = [];
}
