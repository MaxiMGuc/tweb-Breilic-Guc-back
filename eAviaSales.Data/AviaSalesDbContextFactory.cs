using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace eAviaSales.Data;

public class AviaSalesDbContextFactory : IDesignTimeDbContextFactory<AviaSalesDbContext>
{
    public AviaSalesDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AviaSalesDbContext>();
        optionsBuilder.UseSqlServer("Server=DESKTOP-1M9EUHS;Database=eAviaSales;Trusted_Connection=True;TrustServerCertificate=True;");
        return new AviaSalesDbContext(optionsBuilder.Options);
    }
}