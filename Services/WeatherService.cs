using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using WeatherForecastApp.Data;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Services
{
    public class WeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WeatherService> _logger;
        private readonly IntegrationConfigs _integrationConfigs;

        public WeatherService(IHttpClientFactory httpClientFactory, IOptions<AppConfig> configuration, ILogger<WeatherService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _integrationConfigs = configuration.Value.IntegrationConfigs ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
        }

        public async Task<WeatherForecast> GetWeatherForecast(string city, string country, string dateString)
        {
            var weatherForecasts = new WeatherForecast();

            try
            {
                var openWeather = await GetOpenWeatherForecast(city, country, dateString);
                weatherForecasts.OpenWeatherResponse = openWeather;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get forecast from OpenWeather for {City}, {Country}, {Date}.", city, country, dateString);
            }

            try
            {
                var weatherApi = await GetWeatherApiForecast(city, country, dateString);
                weatherForecasts.WeatherApiResponse = weatherApi;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get forecast from WeatherAPI for {City}, {Country}, {Date}.", city, country, dateString);
            }

            try
            {
                var visualCrossing = await GetVisualCrossingForecast(city, country, dateString);
                weatherForecasts.VisualCrossingResponse = visualCrossing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get forecast from VisualCrossing for {City}, {Country}, {Date}.", city, country, dateString);

                if(weatherForecasts.OpenWeatherResponse is null && weatherForecasts.WeatherApiResponse is null && weatherForecasts.VisualCrossingResponse is null)
                    throw new Exception("Could't get data from services.");
            }

            return weatherForecasts;
        }

        private async Task<OpenWeatherDailyResponse?> GetOpenWeatherForecast(string city, string country, string dateString)
        {
            var coordinates = await GetCityCoordinatesAsync(city, country);

            if(coordinates is null)
            {
                _logger.LogError("Coordinates could't find");
                throw new Exception("Unable to fetch coordinate data");
            }

            using var client = _httpClientFactory.CreateClient();

            var url = string.Format(_integrationConfigs.OpenWeather5DayApiUrl, coordinates[0].Latitude, coordinates[0].Longitude);

            var response = await client.GetStringAsync(url);

            var data = JsonConvert.DeserializeObject<OpenWeatherDailyResponse>(response);

            if(!DateTime.TryParse(dateString, out DateTime targetTime))
            {
                throw new ArgumentException("Invalid date format. It should be in YYYY-MM-DD format.");
            }

            var filterDate = targetTime.Date;

            var filteredForecast = data.ForecastList
                .Where(f => DateTime.Parse(f.DtTxt).Date == filterDate)
                .OrderBy(f => Math.Abs((DateTime.Parse(f.DtTxt) - filterDate.AddHours(12)).Ticks)).FirstOrDefault();

            var result = new OpenWeatherDailyResponse { Message = data.Message };

            result.ForecastList.Add(filteredForecast);

            return result;
        }

        public async Task<List<OpenWeatherGeoResponse>> GetCityCoordinatesAsync(string city, string country)
        {
            using var client = new HttpClient();
            var url = string.Format(_integrationConfigs.OpenWeatherGeoApiUrl, city, country);
            var response = await client.GetStringAsync(url);

            var geoCodingResponse = JsonConvert.DeserializeObject<List<OpenWeatherGeoResponse>> (response);

            return geoCodingResponse;
        }


        private async Task<WeatherApiResponse> GetWeatherApiForecast(string city, string country, string dateString)
        {
            using var client = new HttpClient();

            var url = string.Format(_integrationConfigs.WeatherApiUrl, city, country);

            var response = await client.GetStringAsync(url);
            

            var data = JsonConvert.DeserializeObject<WeatherApiResponse>(response);

            if (!DateTime.TryParse(dateString, out DateTime targetTime))
            {
                throw new ArgumentException("Invalid date format. It should be in YYYY-MM-DD format.");
            }

            //filter the forecast for the specific day
            var specificDayForecast = data.Forecast.Forecastday
                .FirstOrDefault(f => f.Date == targetTime.ToString("yyyy-MM-dd"));

            var filteredForecast = new Forecast();
            filteredForecast.Forecastday.Add(specificDayForecast);

            data.Forecast = filteredForecast;

            return data;
        }

        private async Task<VisualCrossingResponse> GetVisualCrossingForecast(string city, string country, string dateString)
        {
            if (!DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                throw new ArgumentException("Invalid date format. It should be in YYYY-MM-DD format.");
            }

            using var client = new HttpClient();
           
            var url = string.Format(_integrationConfigs.VisualCrossing, city, country, dateString);

            var response = await client.GetStringAsync(url);

            var data = JsonConvert.DeserializeObject<VisualCrossingResponse>(response);

            return data;
        }
    }

}