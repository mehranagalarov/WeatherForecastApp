using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherForecastApp.Services
{
    public class WeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<WeatherService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecast(string city, string country, DateTime date)
    {
        var weatherForecasts = new List<WeatherForecast>();

        try
        {
            var openWeather = await GetOpenWeatherForecast(city, country);
            weatherForecasts.Add(openWeather);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get forecast from OpenWeather for {City}, {Country}.", city, country);
        }

        try
        {
            var weatherApi = await GetWeatherApiForecast(city, country);
            weatherForecasts.Add(weatherApi);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get forecast from WeatherAPI for {City}, {Country}.", city, country);
        }

        try
        {
            var visualCrossing = await GetVisualCrossingForecast(city, country);
            weatherForecasts.Add(visualCrossing);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get forecast from VisualCrossing for {City}, {Country}.", city, country);
        }

        return weatherForecasts;
    }

    private async Task<WeatherForecast> GetOpenWeatherForecast(string city, string country)
    {
        var client = _httpClientFactory.CreateClient();
        var apiKey = _configuration["OpenWeatherMapApiKey"];
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city},{country}&appid={apiKey}&units=metric";
        
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<OpenWeatherResponse>();
        return new WeatherForecast
        {
            TemperatureC = data.Main.Temp,
            Summary = data.Weather.FirstOrDefault()?.Description,
            Source = "OpenWeatherMap"
        };
    }

    private async Task<WeatherForecast> GetWeatherApiForecast(string city, string country)
    {
        var client = _httpClientFactory.CreateClient();
        var apiKey = _configuration["WeatherApiKey"];
        var url = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={city},{country}&aqi=no";
        
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<WeatherApiResponse>();
        return new WeatherForecast
        {
            TemperatureC = data.Current.Temp_C,
            Summary = data.Current.Condition.Text,
            Source = "WeatherAPI"
        };
    }

    private async Task<WeatherForecast> GetVisualCrossingForecast(string city, string country)
    {
        var client = _httpClientFactory.CreateClient();
        var apiKey = _configuration["VisualCrossingApiKey"];
        var url = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{city},{country}?unitGroup=metric&key={apiKey}";

        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<VisualCrossingResponse>();
        return new WeatherForecast
        {
            TemperatureC = data.CurrentConditions.Temp,
            Summary = data.CurrentConditions.Conditions,
            Source = "VisualCrossing"
        };
    }
}

}