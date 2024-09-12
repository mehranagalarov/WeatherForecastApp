namespace WeatherForecastApp.Models
{
    public class OpenWeatherDailyResponse
    {
        public int Message { get; set; }
        public List<WeatherForecastList> ForecastList { get; set; } = new List<WeatherForecastList>();
    }
    public class WeatherForecastList
    {
        public long Dt { get; set; }
        public MainWeather Main { get; set; }
        public string DtTxt { get; set; }
    }
    public class MainWeather
    {
        public double Temp { get; set; }
        public double FeelsLike { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
    }
    
}
