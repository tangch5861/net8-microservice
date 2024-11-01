using App.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace MonitoringService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly IMetrics _metrics;

        public MetricsController(IMetrics metrics)
        {
            _metrics = metrics;
        }

        [HttpGet]
        public IActionResult GetMetrics()
        {
            var metricsData = _metrics.Snapshot.Get();
            return Ok(metricsData); // Return metrics as JSON
        }
    }
}
