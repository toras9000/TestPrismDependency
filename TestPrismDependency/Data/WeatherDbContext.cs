using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestPrismDependency.Data;

public class WeatherDbContext : DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }

    public DbSet<WeatherModel> Weathers { get; set; } = default!;
    public DbSet<ForecastModel> Forecasts { get; set; } = default!;
}

public class WeatherModel
{
    public long Id { get; set; }

    public DateTime Time { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Elevation { get; set; }

    public ICollection<ForecastModel>? Forecasts { get; set; }
}

public class ForecastModel
{
    public long Id { get; set; }

    public long WeatherId { get; set; }
    public DateTime Time { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double Rain { get; set; }

    [ForeignKey(nameof(WeatherId))]
    public WeatherModel? Weather { get; set; }
}
