namespace eAviaSales.Api.Services.Orders;

public sealed class OrderRecord
{
    public required string OrderId { get; init; }
    public required string CustomerKey { get; init; }
    public required string Status { get; set; }
    public required IList<OrderLineRecord> Lines { get; init; }
    public decimal TotalAmount { get; init; }
    public required string CurrencyCode { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    /// <summary>Tickets issued after successful payment.</summary>
    public IList<OrderTicketRecord> Tickets { get; init; } = new List<OrderTicketRecord>();
}

public sealed class OrderLineRecord
{
    public int EventId { get; init; }
    public string EventCode { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}

public sealed class OrderTicketRecord
{
    public required string TicketId { get; init; }
    public int EventId { get; init; }
    public string EventCode { get; init; } = string.Empty;
    public required string QrPayload { get; init; }
}
