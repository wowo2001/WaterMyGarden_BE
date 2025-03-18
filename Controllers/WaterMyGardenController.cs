using Microsoft.AspNetCore.Mvc;
using WaterMyGarden.Services;

namespace WaterMyGarden.Controllers
{
    [ApiController]
    [Route("Weather")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly IAwsSnsServices _awsSnsServices;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherService weatherService, IAwsSnsServices awsSnsServices)
        {
            _logger = logger;
            _weatherService = weatherService;
            _awsSnsServices = awsSnsServices;
        }

        [HttpGet("GetWeather")]
        public async Task<ActionResult<Weather>> GetWeather()
        {
            return await _weatherService.GetWeather();
        }

        [HttpPost("RecordWatering")]
        public async Task<ActionResult<string>> RecordWatering([FromBody] RecordWateringRequest request)
        {

            return await _weatherService.RecordWatering(request);

        }

        [HttpGet("SendSNS")]
        public async Task SendSNS()
        { 
            await _awsSnsServices.SendSNS();
        }
    }
}
