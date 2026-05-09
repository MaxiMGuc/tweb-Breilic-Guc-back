namespace eAviaSales.Api.Services.Payments;

public sealed class PaymentRecord
{
    public required string PaymentId { get; init; }
    public required string OrderId { get; init; }
    public decimal Amount { get; init; }
    public required string CurrencyCode { get; init; }
    public required string Status { get; set; }
}
