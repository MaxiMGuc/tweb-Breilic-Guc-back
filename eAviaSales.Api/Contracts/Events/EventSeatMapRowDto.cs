namespace eAviaSales.Api.Contracts.Events;

public sealed class EventSeatMapRowDto
{
    public int RowNumber { get; init; }
    public IReadOnlyList<EventSeatDto> Seats { get; init; } = [];
}
