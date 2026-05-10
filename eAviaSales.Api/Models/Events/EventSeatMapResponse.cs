namespace eAviaSales.Api.Models.Events;

public sealed class EventSeatMapResponse
{
    public int EventId { get; init; }
    public string Layout { get; init; } = "3-3";
    public IReadOnlyList<EventSeatMapRowDto> Rows { get; init; } = [];
}

