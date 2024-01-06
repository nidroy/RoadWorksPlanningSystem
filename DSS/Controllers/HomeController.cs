using DSS.Controllers.ApiControllers;
using DSS.Models;
using DSS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DSS.Controllers
{
    public class HomeController : Controller
    {
        private readonly RoadsApiController _roadsApi;
        private readonly EstimatesApiController _estimatesApi;

        public HomeController(ApplicationContext context, ILogger<RoadsApiController> roadsApiLogger, ILogger<EstimatesApiController> estimatesApiLogger)
        {
            _roadsApi = new RoadsApiController(context, roadsApiLogger);
            _estimatesApi = new EstimatesApiController(context, estimatesApiLogger);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
