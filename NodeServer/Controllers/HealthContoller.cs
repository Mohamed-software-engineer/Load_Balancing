using Microsoft.AspNetCore.Mvc;
using Models;

namespace NodeServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HealthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetHealth()
        {
            var response = new HealthResponse
            {
                NodeId = _configuration["NODE_ID"] ?? "node-unknown",
                NodeName = _configuration["NODE_NAME"] ?? "Node Server",
                Status = "Healthy",
                CheckedAt = DateTime.UtcNow
            };

            return Ok(response);
        }
    }
}