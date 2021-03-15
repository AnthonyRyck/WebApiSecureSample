using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiSecureSample.Data;

namespace WebApiSecureSample.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [Authorize(Policy = "RequestAdmin")]
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
                
        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult GetTest()
        {
            return Ok("Des données pour tout le monde, sans autorisation.");
        }

        [Authorize(Policy = "ManagerRequest")]
        [HttpGet("manager")]
        public IActionResult GetUserData()
        {
            return Ok("C'est des données pour Manager.");
        }

        [Authorize(Policy = "RequestAdmin")]
        [HttpGet("admin")]
        public IActionResult GetAdminData()
        {
            return Ok("C'est des données pour Admin.");
        }

    }
}
