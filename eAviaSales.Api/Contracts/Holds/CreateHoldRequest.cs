using System.ComponentModel.DataAnnotations;

namespace eAviaSales.Api.Contracts.Holds;

public sealed class CreateHoldRequest
{
    [Required]
    [MinLength(1)]
    public IReadOnlyList<string> SeatNumbers { get; init; } = [];

    [Range(1, 30)]
    public int HoldMinutes { get; init; } = 10;
}
