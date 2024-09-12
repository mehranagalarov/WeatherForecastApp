using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json;
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
                _logger.LogError(ex, "Failed to get forecast from OpenWeather for {City}, {Country}.", city, country);
            }

            //try
            //{
            //    var weatherApi = await GetWeatherApiForecast(city, country);
            //    weatherForecasts.Add(weatherApi);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Failed to get forecast from WeatherAPI for {City}, {Country}.", city, country);
            //}

            //try
            //{
            //    var visualCrossing = await GetVisualCrossingForecast(city, country);
            //    weatherForecasts.Add(visualCrossing);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Failed to get forecast from VisualCrossing for {City}, {Country}.", city, country);
            //}

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

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<OpenWeatherDailyResponse>();

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

        public async Task<OpenWeatherGeoResponse[]?> GetCityCoordinatesAsync(string city, string country)
        {
            using var client = new HttpClient();
            var url = string.Format(_integrationConfigs.OpenWeatherGeoApiUrl, city, country);
            var response = await client.GetStringAsync(url);

            var geoCodingResponse = JsonSerializer.Deserialize<OpenWeatherGeoResponse[]>(response);

            return geoCodingResponse;
        }


        //private async Task<WeatherForecast> GetWeatherApiForecast(string city, string country)
        //{
        //    var client = _httpClientFactory.CreateClient();
        //    var apiKey = _configuration["WeatherApiKey"];
        //    var url = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={city},{country}&aqi=no";

        //    var response = await client.GetAsync(url);
        //    response.EnsureSuccessStatusCode();

        //    var data = await response.Content.ReadFromJsonAsync<WeatherApiResponse>();
        //    return new WeatherForecast
        //    {
        //        TemperatureC = data.Current.Temp_C,
        //        Summary = data.Current.Condition.Text,
        //        Source = "WeatherAPI"
        //    };
        //}

        //private async Task<WeatherForecast> GetVisualCrossingForecast(string city, string country)
        //{
        //    var client = _httpClientFactory.CreateClient();
        //    var apiKey = _configuration["VisualCrossingApiKey"];
        //    var url = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{city},{country}?unitGroup=metric&key={apiKey}";

        //    var response = await client.GetAsync(url);
        //    response.EnsureSuccessStatusCode();

        //    var data = await response.Content.ReadFromJsonAsync<VisualCrossingResponse>();
        //    return new WeatherForecast
        //    {
        //        TemperatureC = data.CurrentConditions.Temp,
        //        Summary = data.CurrentConditions.Conditions,
        //        Source = "VisualCrossing"
        //    };
        //}
    }

}