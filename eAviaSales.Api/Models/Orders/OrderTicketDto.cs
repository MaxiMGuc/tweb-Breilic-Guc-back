namespace eAviaSales.Api.Models.Orders;

public sealed class OrderTicketDto
{
    public required string TicketId { get; init; }
    public required string OrderId { get; init; }
    public int EventId { get; init; }
    public string EventCode { get; init; } = string.Empty;
    /// <summary>Opaque payload suitable for QR (MVP).</summary>
    public required string QrPayload { get; init; }
}

