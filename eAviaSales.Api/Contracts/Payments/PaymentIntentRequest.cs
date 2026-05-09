using System.ComponentModel.DataAnnotations;

namespace eAviaSales.Api.Contracts.Payments;

public sealed class PaymentIntentRequest
{
    [Required]
    public required string OrderId { get; init; }
}
