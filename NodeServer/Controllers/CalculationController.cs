using Microsoft.AspNetCore.Mvc;
using Models;
using NodeServer.Services;

namespace Controllers
{
    [ApiController]
    [Route("")]
    public class CalculationController : ControllerBase
    {
        private readonly CalculateH _calculateH;
        private readonly IConfiguration _configuration;

        public CalculationController(CalculateH calculateH, IConfiguration configuration)
        {
            _calculateH = calculateH;
            _configuration = configuration;
        }

        [HttpGet("cal")]
        public IActionResult Calculate([FromQuery] int n)
        {
            var nodeId = _configuration["NODE_ID"] ?? "node-unknown";
            var nodeName = _configuration["NODE_NAME"] ?? "Node Server";

            var (result, limit, processingTimeMs) = _calculateH.Execute(n);

            var response = new CalculationResponse
            {
                NodeId = nodeId,
                NodeName = nodeName,
                N = n,
                Limit = limit,
                Result = result,
                ProcessingTimeMs = processingTimeMs,
                CompletedAt = DateTime.UtcNow
            };

            return Ok(response);
        }
    }
}