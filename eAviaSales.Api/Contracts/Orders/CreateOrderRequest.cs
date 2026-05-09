using System.ComponentModel.DataAnnotations;

namespace eAviaSales.Api.Contracts.Orders;

public sealed class CreateOrderRequest
{
    [Required]
    public required string CheckoutId { get; init; }
}
