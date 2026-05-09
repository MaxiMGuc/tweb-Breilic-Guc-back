namespace eAviaSales.Api.Contracts.Cart;

public sealed class CartItemDto
{
    public string ItemId { get; init; } = string.Empty;
    public int EventId { get; init; }
    public string EventCode { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init; }
    public string CurrencyCode { get; init; } = "USD";
}
