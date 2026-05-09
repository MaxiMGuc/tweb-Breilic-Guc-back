namespace eAviaSales.Api.Services.Orders;

public sealed class CheckoutRecord
{
    public required string CheckoutId { get; init; }
    public required string CustomerKey { get; init; }
    public required IList<OrderLineRecord> Lines { get; init; }
    public decimal TotalAmount { get; init; }
    public required string CurrencyCode { get; init; }
    public DateTime ExpiresAtUtc { get; init; }
}
