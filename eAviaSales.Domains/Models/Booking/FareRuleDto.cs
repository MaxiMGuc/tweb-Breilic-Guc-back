using eAviaSales.Domains.Enums;

namespace eAviaSales.Domains.Models.Booking;

public class FareRuleDto
{
    public int Id { get; set; }
    public FareTier FareTier { get; set; }
    public int CheckedBagsIncluded { get; set; }
    public int CarryOnWeightKg { get; set; }
    public int CheckedBagWeightKg { get; set; }
    public decimal PriceMultiplier { get; set; }
    public string Summary { get; set; } = string.Empty;
}
