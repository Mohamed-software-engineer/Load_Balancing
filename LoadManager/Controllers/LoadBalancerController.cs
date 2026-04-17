using Services;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoadBalancerController : ControllerBase
    {
        private readonly LoadBalancerService _loadBalancerService;

        public LoadBalancerController(LoadBalancerService loadBalancerService)
        {
            _loadBalancerService = loadBalancerService;
        }

        [HttpGet("cal")]
        public async Task<IActionResult> Calculate([FromQuery] int n, [FromQuery] string algo = "rr")
        {
            var result = await _loadBalancerService.ForwardRequestAsync(n, algo);
            return Ok(result);
        }
    }
}