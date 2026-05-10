namespace eAviaSales.Api.Models.Events;

public sealed class EventSeatMapRowDto
{
    public int RowNumber { get; init; }
    public IReadOnlyList<EventSeatDto> Seats { get; init; } = [];
}

