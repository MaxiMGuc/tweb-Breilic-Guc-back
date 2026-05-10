using System.ComponentModel.DataAnnotations;

namespace eAviaSales.Api.Models.Orders;

public sealed class CreateOrderRequest
{
    [Required]
    public required string CheckoutId { get; init; }
}

