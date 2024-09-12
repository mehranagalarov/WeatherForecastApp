using Microsoft.AspNetCore.Mvc;
using WeatherForecastApp.Services.Interfaces;

namespace WeatherForecastApp.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherForecastController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("getWeatherForecast")]
    public async Task<IActionResult> GetWeatherForecast(string city, string country, string date)
    {
        return Ok(await _weatherService.GetWeatherForecast(city, country, date));
    }
}
