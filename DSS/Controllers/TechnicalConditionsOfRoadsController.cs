using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using DSS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DSS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("technicalConditionsOfRoads")]
    public class TechnicalConditionsOfRoadsController : Controller
    {
        private readonly RoadsApiController _roadsApi;
        private readonly TechnicalConditionsOfRoadsApiController _technicalConditionsOfRoadsApi;
        private readonly ApiLogger _logger;

        public TechnicalConditionsOfRoadsController(ApplicationContext context, ILogger<ApiController> logger)
        {
            _roadsApi = new RoadsApiController(context, logger);
            _technicalConditionsOfRoadsApi = new TechnicalConditionsOfRoadsApiController(context, logger);
            _logger = new ApiLogger(logger);
        }

        [HttpGet("read")]
        public IActionResult Read()
        {
            try
            {
                _logger.LogInformation("TechnicalConditionsOfRoadsController/Read", "Reading all technical conditions of roads...");

                var result = _technicalConditionsOfRoadsApi.Get();
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("TechnicalConditionsOfRoadsController/Read", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var technicalConditionsOfRoads = JsonConvert.DeserializeObject<IEnumerable<TechnicalConditionOfRoad>>(value.ToString());

                _logger.LogInformation("TechnicalConditionsOfRoadsController/Read", "All technical conditions of roads have been successfully read.");

                _logger.LogInformation($"TechnicalConditionsOfRoadsController/Read", "Reading all roads...");

                result = _roadsApi.Get();
                statusCode = ((ObjectResult)result).StatusCode;
                value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"TechnicalConditionsOfRoadsController/Read", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var roads = JsonConvert.DeserializeObject<IEnumerable<Road>>(value.ToString());

                _logger.LogInformation($"TechnicalConditionsOfRoadsController/Read", "All roads have been successfully read.");

                List<RoadTechnicalConditionsOfRoadsViewModel> viewModels = new();

                foreach (var road in roads)
                {
                    var roadTechnicalConditionsOfRoads = technicalConditionsOfRoads
                        .Where(tc => tc.RoadId == road.Id)
                        .GroupBy(tc => tc.Year)
                        .ToDictionary(tc => tc.Key, tc => tc.AsEnumerable());

                    RoadTechnicalConditionsOfRoadsViewModel viewModel = new()
                    {
                        Road = road,
                        TechnicalConditionsOfRoads = roadTechnicalConditionsOfRoads
                    };

                    viewModels.Add(viewModel);
                }

                _logger.LogInformation("TechnicalConditionsOfRoadsController", "Navigating to the page \"Read All Technical Conditions Of Roads\".");

                return View("Index", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError("TechnicalConditionsOfRoadsController", $"Error when navigating to the page \"Read All Technical Conditions Of Roads\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
