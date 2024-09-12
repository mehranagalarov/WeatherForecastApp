using Newtonsoft.Json;

namespace WeatherForecastApp.Models
{
    public class WeatherApiResponse
    {
        public Location Location { get; set; }
        public Forecast Forecast { get; set; }
    }

    public class Location
    {
        public string Name { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
    public class Forecast
    {
        public List<ForecastDay> Forecastday { get; set; } = new List<ForecastDay>();
    }
    public class ForecastDay
    {
        public string Date { get; set; }
        public Day Day { get; set; }
    }
    public class Day
    {
        [JsonProperty(PropertyName ="maxtemp_c")]
        public double MaxtempC { get; set; }
        [JsonProperty(PropertyName = "mintemp_c")]
        public double MintempC { get; set; }
        [JsonProperty(PropertyName = "avgtemp_c")]
        public double AvgTempC { get; set; }

    }
   
    
}
