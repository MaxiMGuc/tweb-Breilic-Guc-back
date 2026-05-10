namespace eAviaSales.Api.Models.Orders;

public sealed class OrderLineDto
{
    public int EventId { get; init; }
    public string EventCode { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}

