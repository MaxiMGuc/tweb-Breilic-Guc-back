namespace eAviaSales.Api.Models.Events;

public sealed class EventSummaryDto
{
    public int EventId { get; init; }
    public string EventCode { get; init; } = string.Empty;
    public string ProviderName { get; init; } = string.Empty;
    public string FromIataCode { get; init; } = string.Empty;
    public string ToIataCode { get; init; } = string.Empty;
    public DateTime StartsAtUtc { get; init; }
    public DateTime EndsAtUtc { get; init; }
    public decimal Price { get; init; }
    public string CurrencyCode { get; init; } = "USD";
    public string AvailabilityStatus { get; init; } = string.Empty;
}

