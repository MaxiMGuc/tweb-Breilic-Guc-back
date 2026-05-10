using System.ComponentModel.DataAnnotations;

namespace eAviaSales.Api.Models.Payments;

public sealed class PaymentIntentRequest
{
    [Required]
    public required string OrderId { get; init; }
}

