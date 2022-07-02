using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TestPrismDependency.Data;

namespace TestPrismDependency.Design;

/// <summary>
/// ef tools向けのデザイン時DbContextファクトリ
/// </summary>
public class WeatherDbContextDesignTimeFactory : IDesignTimeDbContextFactory<WeatherDbContext>
{
    public WeatherDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WeatherDbContext>();
        optionsBuilder.UseSqlite("Data Source=:memory:");

        return new WeatherDbContext(optionsBuilder.Options);
    }
}