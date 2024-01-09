using DSS.Loggers;
using DSS.Models;
using Microsoft.AspNetCore.Mvc;

namespace DSS.Controllers.ApiControllers
{
    [ApiController]
    [Route("api/technicalConditionsOfRoads")]
    public class TechnicalConditionsOfRoadsApiController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ApiLogger _logger;

        public TechnicalConditionsOfRoadsApiController(ApplicationContext context, ILogger<TechnicalConditionsOfRoadsApiController> logger)
        {
            _context = context;
            _logger = new ApiLogger(logger);
        }

        [HttpGet("get")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
