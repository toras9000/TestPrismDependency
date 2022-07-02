using System;

namespace TestPrismDependency.Data;

public record Forecast(DateTime Time, double Temperature, double Humidity, double Rain);
public record Weather(double Latitude, double Longitude, double Elevation, Forecast[] Forecasts);
