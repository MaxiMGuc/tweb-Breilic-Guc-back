using eAviaSales.Domains.Enums;

namespace eAviaSales.Domains.Models.Booking;

public class BookingDto
{
    public int Id { get; set; }
    public int FlightId { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public FareTier FareTier { get; set; }
    public BookingStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public DateTime BookedAtUtc { get; set; }
    public FareRuleDto AppliedFareRule { get; set; } = new();
    public List<BookingPassengerDto> Passengers { get; set; } = [];
}
