using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherForecastApp.Services
{
    public class CachedWeatherService : IWeatherService
    {
        private readonly WeatherService _weatherService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachedWeatherService> _logger;

        public CachedWeatherService(WeatherService weatherService, IMemoryCache cache, ILogger<CachedWeatherService> logger)
        {
            _weatherService = weatherService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecast(string city, string country, DateTime date)
        {
            string cacheKey = $"{city}-{country}-{date.ToShortDateString()}";
            IEnumerable<WeatherForecast> forecasts = null;

            try
            {
                // Attempt to get fresh data from the API
                forecasts = await _weatherService.GetWeatherForecast(city, country, date);

                // Store the fetched forecast in the cache for future use
                _cache.Set(cacheKey, forecasts, TimeSpan.FromHours(1));

                _logger.LogInformation("Successfully fetched weather forecast from APIs for {City}, {Country} on {Date}.", city, country, date);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "API call failed. Attempting to retrieve cached forecast for {City}, {Country} on {Date}.", city, country, date);

                // If the API call fails, attempt to retrieve the forecast from the cache
                if (_cache.TryGetValue(cacheKey, out forecasts))
                {
                    _logger.LogWarning("Returning cached forecast for {City}, {Country} on {Date} due to API failure.", city, country, date);
                    return forecasts;
                }
                else
                {
                    // If no cached data is available, throw an exception
                    _logger.LogError("No cached data available for {City}, {Country} on {Date} and API call failed.", city, country, date);
                    throw new Exception("Unable to fetch weather data and no cached data is available.");
                }
            }

            return forecasts;
        }
    }


}