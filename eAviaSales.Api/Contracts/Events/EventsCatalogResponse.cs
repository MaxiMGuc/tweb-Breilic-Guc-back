namespace eAviaSales.Api.Contracts.Events;

public sealed class EventsCatalogResponse
{
    public IReadOnlyList<EventSummaryDto> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
}
