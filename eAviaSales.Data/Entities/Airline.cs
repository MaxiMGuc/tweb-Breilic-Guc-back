namespace eAviaSales.Data.Entities;

public class Airline
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
