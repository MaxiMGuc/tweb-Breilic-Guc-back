using System.ComponentModel.DataAnnotations;

namespace eAviaSales.Domains.Models.Flight;

public class FlightSearchRequest
{
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string FromIataCode { get; set; } = string.Empty;

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string ToIataCode { get; set; } = string.Empty;

    public DateTime DepartureDateUtc { get; set; }

    [Range(1, 9)]
    public int Adults { get; set; }
}
