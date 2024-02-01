using Microsoft.AspNetCore.Mvc;

namespace WorkerApi.Controllers
{
    [ApiController]
    [Route( "[controller]" )]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController( ILogger<WeatherForecastController> logger )
        {
            _logger = logger;
        }

        [HttpGet( Name = "GetWeatherForecast" )]
        public string Get( [FromQuery] string id)
        {
            return id;
        }
    }
}