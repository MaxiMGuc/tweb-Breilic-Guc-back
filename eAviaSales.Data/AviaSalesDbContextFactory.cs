using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace eAviaSales.Data;

public class AviaSalesDbContextFactory : IDesignTimeDbContextFactory<AviaSalesDbContext>
{
    public AviaSalesDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AviaSalesDbContext>();
        optionsBuilder.UseSqlite("Data Source=eAviaSales_design.db");
        return new AviaSalesDbContext(optionsBuilder.Options);
    }
}
