using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherForecastApp.Models;

namespace WeatherForecastApp.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherForecast> GetWeatherForecast(string city, string country, string date);
    }
}