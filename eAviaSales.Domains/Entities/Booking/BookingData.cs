using eAviaSales.Domains.Enums;

namespace eAviaSales.Domains.Entities.Booking;

public class BookingData
{
    public int Id { get; set; }
    public int FlightId { get; set; }
    public FareTier FareTier { get; set; }
    public BookingStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public DateTime BookedAtUtc { get; set; }
    public List<BookingPassengerData> Passengers { get; set; } = [];
}
