using DSS.Controllers.ApiControllers;
using DSS.Models;
using DSS.Models.ViewModels;
using DSS.Modules;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DSS.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeApiController _homeApi;

        public HomeController(ApplicationContext context, ILogger<ApiController> logger)
        {
            _homeApi = new HomeApiController(context, logger);
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
