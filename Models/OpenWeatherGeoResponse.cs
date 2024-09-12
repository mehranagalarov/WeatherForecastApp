using Newtonsoft.Json;

namespace WeatherForecastApp.Models
{
    public class OpenWeatherGeoResponse
    {
        [JsonProperty(PropertyName ="lat")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "lon")]
        public double Longitude { get; set; }
    }
}
