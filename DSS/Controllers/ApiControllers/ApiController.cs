using DSS.Loggers;
using DSS.Models;
using Microsoft.AspNetCore.Mvc;

namespace DSS.Controllers.ApiControllers
{
    public abstract class ApiController : ControllerBase
    {
        protected readonly ApplicationContext _context;
        protected readonly ApiLogger _logger;

        public ApiController(ApplicationContext context, ILogger<ApiController> logger)
        {
            _context = context;
            _logger = new ApiLogger(logger);
        }
    }
}
