namespace eAviaSales.Domains.Entities.Booking;

public class BookingPassengerData
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirthUtc { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
}
