using eAviaSales.Domains.Enums;

namespace eAviaSales.Data.Entities;

public class Booking
{
    public int Id { get; set; }
    public int FlightId { get; set; }
    public Flight Flight { get; set; } = null!;
    public FareTier FareTier { get; set; }
    public BookingStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public DateTime BookedAtUtc { get; set; }
    public ICollection<BookingPassenger> Passengers { get; set; } = new List<BookingPassenger>();
}
