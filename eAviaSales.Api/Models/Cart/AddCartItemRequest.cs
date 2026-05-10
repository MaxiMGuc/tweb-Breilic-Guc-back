using System.ComponentModel.DataAnnotations;

namespace eAviaSales.Api.Models.Cart;

public sealed class AddCartItemRequest
{
    [Range(1, int.MaxValue)]
    public int EventId { get; init; }

    [Range(1, 20)]
    public int Quantity { get; init; } = 1;
}

