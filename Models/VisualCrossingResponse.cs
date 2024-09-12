namespace WeatherForecastApp.Models
{
    public class VisualCrossingResponse
    {
        public List<WeatherDay> Days { get; set; } = new List<WeatherDay>();
    }
    public class WeatherDay
    {
        public string Datetime { get; set; }
        public double Tempmax { get; set; }
        public double Tempmin { get; set; }
        public double Temp { get; set; }
        public double Feelslike { get; set; }
    }
}
