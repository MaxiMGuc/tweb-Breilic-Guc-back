namespace eAviaSales.Data.Entities;

public class Airport
{
    public int Id { get; set; }
    public string IataCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
