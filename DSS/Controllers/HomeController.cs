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
        private readonly TechnicalConditionsOfRoadsApiController _technicalConditionsOfRoadsApi;
        private readonly HomeApiController _homeApi;

        public HomeController(ApplicationContext context, ILogger<ApiController> logger)
        {
            _roadsApi = new RoadsApiController(context, logger);
            _estimatesApi = new EstimatesApiController(context, logger);
            _technicalConditionsOfRoadsApi = new TechnicalConditionsOfRoadsApiController(context, logger);
            _homeApi = new HomeApiController(context, logger);
        }

        public IActionResult Index()
        {
            _homeApi.GetPlans();
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
