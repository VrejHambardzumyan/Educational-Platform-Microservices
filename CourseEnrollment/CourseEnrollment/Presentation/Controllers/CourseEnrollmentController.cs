using Microsoft.AspNetCore.Mvc;

namespace CourseEnrollment.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseEnrollmentController : ControllerBase
    {
        private readonly ILogger<CourseEnrollmentController> _logger;

        private readonly IEnrollment _enrollment;
        public CourseEnrollmentController(ILogger<CourseEnrollmentController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
