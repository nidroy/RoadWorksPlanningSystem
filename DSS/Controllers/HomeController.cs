using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using DSS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DSS.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeApiController _homeApi;
        private readonly ApiLogger _logger;

        public HomeController(ApplicationContext context, ILogger<ApiController> logger)
        {
            _homeApi = new HomeApiController(context, logger);
            _logger = new ApiLogger(logger);
        }

        private static List<(int, string, Dictionary<int, List<Estimate>>)>? plans = null;

        private static InputDataViewModel viewModel = new()
        {
            InitialYear = 0,
            InitialMonth = "Январь",
            YearCount = 0,
            Budget = 0,
            Months = new List<string>
            {
                "Январь",
                "Февраль",
                "Март",
                "Апрель",
                "Май",
                "Июнь",
                "Июль",
                "Август",
                "Сентябрь",
                "Октябрь",
                "Ноябрь",
                "Декабрь"
            }
        };

        public IActionResult Index()
        {
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost("planning")]
        public IActionResult Planning(InputDataViewModel inputData)
        {
            try
            {
                _logger.LogInformation("HomeController/Planning", "Planning...");

                viewModel = new()
                {
                    InitialYear = inputData.InitialYear,
                    InitialMonth = inputData.InitialMonth,
                    YearCount = inputData.YearCount,
                    Budget = inputData.Budget,
                    Months = viewModel.Months
                };

                var result = _homeApi.GetPlans(viewModel);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("HomeController/Planning", "Error on the API side of the controller.");
                    return BadRequest(value);
                }

                plans = JsonConvert.DeserializeObject<List<(int, string, Dictionary<int, List<Estimate>>)>>(value.ToString());

                _logger.LogInformation("HomeController/Planning", "The planning has been successfully carried.");

                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("HomeController/Planning", $"Planning error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("statistics")]
        public IActionResult Statistics()
        {
            try
            {
                _logger.LogInformation("HomeController/Statistics", "Getting statistics...");

                var result = _homeApi.GetStatistics(viewModel.Budget, plans);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("HomeController/Statistics", "Error on the API side of the controller.");
                    return BadRequest(value);
                }

                var statistics = JsonConvert.DeserializeObject<StatisticsViewModel>(value.ToString());

                _logger.LogInformation("HomeController/Statistics", "Statistics have been successfully received.");

                _logger.LogInformation("HomeController", "Navigating to the page \"Statistics\".");

                return View("Statistics", statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError("HomeController", $"Error when navigating to the page \"Statistics\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
