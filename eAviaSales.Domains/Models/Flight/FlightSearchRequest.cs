namespace eAviaSales.Domains.Models.Flight;

public class FlightSearchRequest
{
    public string FromIataCode { get; set; } = string.Empty;
    public string ToIataCode { get; set; } = string.Empty;
    public DateTime DepartureDateUtc { get; set; }
    public int Adults { get; set; }
}
