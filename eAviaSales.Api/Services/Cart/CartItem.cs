namespace eAviaSales.Api.Services.Cart;

public sealed class CartItem
{
    public string ItemId { get; init; } = string.Empty;
    public int EventId { get; init; }
    public string EventCode { get; init; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; init; }
    public string CurrencyCode { get; init; } = "USD";
}
