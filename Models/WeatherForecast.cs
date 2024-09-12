namespace WeatherForecastApp.Models
{
    public class WeatherForecast
    {
        public OpenWeatherDailyResponse? OpenWeatherResponse { get; set; }

        public WeatherApiResponse? WeatherApiResponse { get; set; }

        public VisualCrossingResponse? VisualCrossingResponse { get; set;}
    }
}
