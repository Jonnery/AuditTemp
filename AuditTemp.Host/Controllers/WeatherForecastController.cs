using Audit.Realization.Auditing;
using Audit.Realization.Models;
using Audit.Realization.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuditTemp.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Audited]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IAuditLogService _auditLogService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IAuditLogService auditLogService)
        {
            _logger = logger;
            _auditLogService = auditLogService;
        }

        [HttpGet("GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetListStr")]
        [DisableAuditing]
        public async Task<List<AuditLog>> GetList(string serviceName, string ip, int pageindex, int pageSize)
        {
            return await _auditLogService.GetByWhereListAsync(serviceName, ip, pageindex, pageSize);
        }
    }
}