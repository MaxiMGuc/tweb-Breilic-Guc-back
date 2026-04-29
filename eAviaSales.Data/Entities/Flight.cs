using eAviaSales.Domains.Enums;

namespace eAviaSales.Data.Entities;

public class Flight
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public int AirlineId { get; set; }
    public Airline Airline { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int SeatsAvailable { get; set; }
    public FlightStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public ICollection<FlightSegment> Segments { get; set; } = new List<FlightSegment>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
