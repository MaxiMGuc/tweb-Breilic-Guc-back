namespace eAviaSales.Data.Entities;

public class BookingPassenger
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirthUtc { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
}
