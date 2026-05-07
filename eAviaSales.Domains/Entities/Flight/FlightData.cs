using eAviaSales.Domains.Entities.Refs;
using eAviaSales.Domains.Enums;

namespace eAviaSales.Domains.Entities.Flight;

public class FlightData : AuditableEntity
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public AirlineData Airline { get; set; } = new();
    public List<FlightSegmentData> Segments { get; set; } = [];
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int SeatsAvailable { get; set; }
    public FlightStatus Status { get; set; }
}
