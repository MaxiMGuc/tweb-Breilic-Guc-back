namespace eAviaSales.Api.Services.Cart;

public sealed class CartSnapshot
{
    public string CartId { get; init; } = string.Empty;
    public IReadOnlyList<CartItem> Items { get; init; } = [];
}
