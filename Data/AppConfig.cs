namespace WeatherForecastApp.Data
{
    public class AppConfig
    {
        public IntegrationConfigs? IntegrationConfigs { get; set; }
    }

    public class IntegrationConfigs
    {
        public string OpenWeather5DayApiUrl { get; set; }
        public string OpenWeatherGeoApiUrl { get; set; }
        public string WeatherApiUrl { get; set; }
        public string VisualCrossing { get; set; }
    }
}
