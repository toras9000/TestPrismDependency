using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestPrismDependency.Data;

namespace TestPrismDependency.Services;

public interface IWeatherRecorder
{
    Task SaveWeatherAsync(DateTime time, Weather weather);
}

public class EfCoreWeatherRecorder : IWeatherRecorder
{
    public EfCoreWeatherRecorder(IDbContextFactory<WeatherDbContext> dbFactory)
    {
        this.dbFactory = dbFactory;
    }

    public async Task SaveWeatherAsync(DateTime time, Weather weather)
    {
        if (weather == null) throw new ArgumentNullException(nameof(weather));

        using var db = await this.dbFactory.CreateDbContextAsync().ConfigureAwait(false);

        var weatherEntity = new WeatherModel();
        weatherEntity.Time = time.ToUniversalTime();
        weatherEntity.Latitude = weather.Latitude;
        weatherEntity.Longitude = weather.Longitude;
        weatherEntity.Elevation = weather.Elevation;
        db.Weathers.Add(weatherEntity);

        foreach (var forecast in weather.Forecasts)
        {
            var forecastEntity = new ForecastModel();

            forecastEntity.Weather = weatherEntity;
            forecastEntity.Time = forecast.Time;
            forecastEntity.Temperature = forecast.Temperature;
            forecastEntity.Humidity = forecast.Humidity;
            forecastEntity.Rain = forecast.Rain;
            db.Forecasts.Add(forecastEntity);
        }


        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    private readonly IDbContextFactory<WeatherDbContext> dbFactory;
}
