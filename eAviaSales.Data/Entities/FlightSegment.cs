namespace eAviaSales.Data.Entities;

public class FlightSegment
{
    public int Id { get; set; }
    public int FlightId { get; set; }
    public Flight Flight { get; set; } = null!;
    public int DepartureAirportId { get; set; }
    public Airport DepartureAirport { get; set; } = null!;
    public int ArrivalAirportId { get; set; }
    public Airport ArrivalAirport { get; set; } = null!;
    public DateTime DepartureAtUtc { get; set; }
    public DateTime ArrivalAtUtc { get; set; }
    public int SegmentOrder { get; set; }
}
