using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestPrismDependency.Data;

namespace TestPrismDependency.Services;

public interface IWeatherRecorderMigrator
{
    Task MigrateAsync();
}

public class EfCoreWeatherRecorderMigrator : IWeatherRecorderMigrator
{
    public EfCoreWeatherRecorderMigrator(IDbContextFactory<WeatherDbContext> dbFactory)
    {
        this.dbFactory = dbFactory;
    }

    public async Task MigrateAsync()
    {
        using var db = await this.dbFactory.CreateDbContextAsync().ConfigureAwait(false);

        await db.Database.MigrateAsync().ConfigureAwait(false);
    }

    private readonly IDbContextFactory<WeatherDbContext> dbFactory;
}
