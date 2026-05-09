namespace eAviaSales.Api.Contracts.Orders;

public sealed class OrderResponse
{
    public required string OrderId { get; init; }
    public required string Status { get; init; }
    public IReadOnlyList<OrderLineDto> Lines { get; init; } = [];
    public decimal TotalAmount { get; init; }
    public required string CurrencyCode { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
