using eAviaSales.Domains.Enums;

namespace eAviaSales.Domains.Models.Booking;

public class CreateBookingRequest
{
    public int FlightId { get; set; }
    public FareTier FareTier { get; set; }
    public List<BookingPassengerInput> Passengers { get; set; } = [];
}
