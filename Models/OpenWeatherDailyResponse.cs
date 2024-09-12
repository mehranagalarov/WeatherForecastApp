using Newtonsoft.Json;

namespace WeatherForecastApp.Models
{
    public class OpenWeatherDailyResponse
    {
        public int Message { get; set; }
        [JsonProperty(PropertyName ="list")]
        public List<WeatherForecastList> ForecastList { get; set; } = new List<WeatherForecastList>();
    }
    public class WeatherForecastList
    {
        public long Dt { get; set; }
        public MainWeather Main { get; set; }
        [JsonProperty(PropertyName = "dt_txt")]
        public string DtTxt { get; set; }
    }
    public class MainWeather
    {
        public double Temp { get; set; }
        [JsonProperty(PropertyName = "feels_like")]
        public double FeelsLike { get; set; }
        [JsonProperty(PropertyName = "temp_min")]
        public double TempMin { get; set; }
        [JsonProperty(PropertyName = "temp_max")]
        public double TempMax { get; set; }
    }
    
}
