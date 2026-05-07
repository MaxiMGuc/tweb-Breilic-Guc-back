namespace eAviaSales.Domains.Entities.Flight;

public class FlightSegmentData
{
    public int Id { get; set; }
    public AirportData DepartureAirport { get; set; } = new();
    public AirportData ArrivalAirport { get; set; } = new();
    public DateTime DepartureAtUtc { get; set; }
    public DateTime ArrivalAtUtc { get; set; }
}
