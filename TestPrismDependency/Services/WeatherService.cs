using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace TestPrismDependency.Services;

public record Forecast(DateTime Time, double Temperature, double Humidity, double Rain);
public record Weather(double Latitude, double Longitude, double Elevation, Forecast[] Forecasts);

public interface IWeatherService
{
    public Task<Weather> GetWeatherAsync(double latitude, double longitude);
}

public class OpenMeteoWeatherService : IWeatherService
{
    public OpenMeteoWeatherService(IHttpClientFactory httpFactory)
    {
        this.client = httpFactory.CreateClient();
    }

    public async Task<Weather> GetWeatherAsync(double latitude, double longitude)
    {
        var reqUri = $"{ApiBaseUri}?latitude={latitude}&longitude={longitude}&hourly=temperature_2m,relativehumidity_2m,rain";
        var result = await this.client.GetFromJsonAsync<ResultData>(reqUri).ConfigureAwait(false);
        if (result.error != null
         || result.hourly.time.Length != result.hourly.temperature_2m.Length
         || result.hourly.time.Length != result.hourly.relativehumidity_2m.Length
         || result.hourly.time.Length != result.hourly.rain.Length)
        {
            throw new Exception("Failed: " + result.reason);
        }

        var weather = new Weather(
            result.latitude,
            result.longitude,
            result.elevation,
            result.hourly.time.Select((t, i) => new Forecast(t, result.hourly.temperature_2m[i], result.hourly.relativehumidity_2m[i], result.hourly.rain[i])).ToArray()
        );
        return weather;
    }

    private record ResultHourly(DateTime[] time, double[] temperature_2m, double[] relativehumidity_2m, double[] rain);
    private record ResultData(string error, string reason, double latitude, double longitude, double elevation, ResultHourly hourly);

    private const string ApiBaseUri = "https://api.open-meteo.com/v1/forecast";
    private HttpClient client;
}
