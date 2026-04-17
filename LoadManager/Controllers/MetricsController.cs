using Services;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly MetricsService _metricsService;
        private readonly NodeRegistryService _nodeRegistryService;

        public MetricsController(MetricsService metricsService, NodeRegistryService nodeRegistryService)
        {
            _metricsService = metricsService;
            _nodeRegistryService = nodeRegistryService;
        }

        [HttpGet]
        public IActionResult GetMetrics()
        {
            var metrics = _metricsService.GetMetrics(_nodeRegistryService.GetNodes());
            return Ok(metrics);
        }

        [HttpPost("reset")]
        public IActionResult ResetMetrics()
        {
            _metricsService.Reset(_nodeRegistryService.GetNodes());
            return Ok(new { message = "Metrics reset successfully." });
        }
    }
}