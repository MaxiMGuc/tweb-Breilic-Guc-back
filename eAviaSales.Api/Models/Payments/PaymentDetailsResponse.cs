namespace eAviaSales.Api.Models.Payments;

public sealed class PaymentDetailsResponse
{
    public required string PaymentId { get; init; }
    public required string OrderId { get; init; }
    public decimal Amount { get; init; }
    public required string CurrencyCode { get; init; }
    public required string Status { get; init; }
}

