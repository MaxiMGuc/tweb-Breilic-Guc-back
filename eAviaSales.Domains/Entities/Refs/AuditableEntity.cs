namespace eAviaSales.Domains.Entities.Refs;

public class AuditableEntity
{
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
