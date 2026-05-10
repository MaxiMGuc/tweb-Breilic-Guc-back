namespace eAviaSales.Api.Models.Cart;

public sealed class CartResponse
{
    public string CartId { get; init; } = string.Empty;
    public IReadOnlyList<CartItemDto> Items { get; init; } = [];
    public decimal GrandTotal { get; init; }
    public string CurrencyCode { get; init; } = "USD";
}

