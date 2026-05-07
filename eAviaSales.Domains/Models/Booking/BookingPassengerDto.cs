namespace eAviaSales.Domains.Models.Booking;

public class BookingPassengerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirthUtc { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
}
