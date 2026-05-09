namespace eAviaSales.Api.Contracts.Orders;

public sealed class CheckoutResponse
{
    public required string CheckoutId { get; init; }
    public IReadOnlyList<OrderLineDto> Lines { get; init; } = [];
    public decimal TotalAmount { get; init; }
    public required string CurrencyCode { get; init; }
    public DateTime ExpiresAtUtc { get; init; }
}
