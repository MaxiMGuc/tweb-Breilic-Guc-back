namespace eAviaSales.Domains.Models.Flight;

public class FlightTicketDto
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string AirlineName { get; set; } = string.Empty;
    public string FromIataCode { get; set; } = string.Empty;
    public string ToIataCode { get; set; } = string.Empty;
    public DateTime DepartureAtUtc { get; set; }
    public DateTime ArrivalAtUtc { get; set; }
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int SeatsAvailable { get; set; }
    public string Status { get; set; } = string.Empty;
}
