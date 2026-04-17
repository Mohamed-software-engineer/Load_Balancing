using Services;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NodesController : ControllerBase
    {
        private readonly NodeRegistryService _nodeRegistryService;

        public NodesController(NodeRegistryService nodeRegistryService)
        {
            _nodeRegistryService = nodeRegistryService;
        }

        [HttpGet]
        public IActionResult GetNodes()
        {
            return Ok(_nodeRegistryService.GetNodes());
        }
    }
}