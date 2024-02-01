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

                _logger.LogInformation("TechnicalConditionsOfRoadsController/Read", "Reading all roads...");

                result = _roadsApi.Get();
                statusCode = ((ObjectResult)result).StatusCode;
                value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("TechnicalConditionsOfRoadsController/Read", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var roads = JsonConvert.DeserializeObject<IEnumerable<Road>>(value.ToString());

                _logger.LogInformation("TechnicalConditionsOfRoadsController/Read", "All roads have been successfully read.");

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

                _logger.LogInformation("TechnicalConditionsOfRoadsController", "Navigating to the page \"Read All Technical Conditions Of Roads By Year\".");

                return View("Index", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError("TechnicalConditionsOfRoadsController", $"Error when navigating to the page \"Read All Technical Conditions Of Roads By Year\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("read/{roadId}/{year}")]
        public IActionResult Read(int roadId, int year)
        {
            try
            {
                _logger.LogInformation($"TechnicalConditionsOfRoadsController/Read/{roadId}/{year}", $"Reading the technical conditions of road for {year}...");

                var result = _technicalConditionsOfRoadsApi.Get();
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"TechnicalConditionsOfRoadsController/Read/{roadId}/{year}", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var technicalConditionsOfRoads = JsonConvert.DeserializeObject<IEnumerable<TechnicalConditionOfRoad>>(value.ToString());

                _logger.LogInformation($"TechnicalConditionsOfRoadsController/Read/{roadId}/{year}", $"The technical conditions of road for {year} was successfully read.");

                var technicalConditionsOfRoadsForYear = technicalConditionsOfRoads
                    .Where(tc => tc.RoadId == roadId && tc.Year == year)
                    .ToList();

                _logger.LogInformation("TechnicalConditionsOfRoadsController", $"Navigating to the page \"Read Technical Conditions Of Road For {year}\".");

                return View("Read", technicalConditionsOfRoadsForYear);
            }
            catch (Exception ex)
            {
                _logger.LogError("TechnicalConditionsOfRoadsController", $"Error when navigating to the page \"Read Technical Conditions Of Road For {year}\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            try
            {
                _logger.LogInformation("TechnicalConditionsOfRoadsController/Create", "Reading all roads...");

                var result = _roadsApi.Get();
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("TechnicalConditionsOfRoadsController/Create", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var roads = JsonConvert.DeserializeObject<IEnumerable<Road>>(value.ToString());

                _logger.LogInformation("TechnicalConditionsOfRoadsController/Create", "All roads have been successfully read.");

                TechnicalConditionOfRoadRoadsViewModel viewModel = new()
                {
                    TechnicalConditionOfRoad = new(),
                    Roads = roads,
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

                _logger.LogInformation("TechnicalConditionsOfRoadsController", "Navigating to the page \"Create Technical Condition Of Road\".");

                return View("Create", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("TechnicalConditionsOfRoadsController", $"Error when navigating to the page \"Create Technical Condition Of Road\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("create")]
        public IActionResult Create(TechnicalConditionOfRoad technicalConditionOfRoad)
        {
            try
            {
                _logger.LogInformation("TechnicalConditionsOfRoadsController/Create", "Creating a new technical condition of road...");

                if (technicalConditionOfRoad == null)
                {
                    _logger.LogWarning("TechnicalConditionsOfRoadsController/Create", "Incorrect technical condition of road data provided.");
                    return BadRequest("Incorrect technical condition of road data provided");
                }

                TechnicalConditionOfRoadViewModel technicalConditionOfRoadData = new()
                {
                    Year = technicalConditionOfRoad.Year,
                    Month = technicalConditionOfRoad.Month,
                    TechnicalCondition = technicalConditionOfRoad.TechnicalCondition,
                    RoadId = technicalConditionOfRoad.RoadId,
                };

                var result = _technicalConditionsOfRoadsApi.Post(technicalConditionOfRoadData);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("TechnicalConditionsOfRoadsController/Create", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                _logger.LogInformation("TechnicalConditionsOfRoadsController/Create", $"A new technical condition of road with Id {value} has been successfully created.");

                return RedirectToAction("Read");
            }
            catch (Exception ex)
            {
                _logger.LogError("TechnicalConditionsOfRoadsController/Create", $"Error when creating a new technical condition of road: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("update/{id}")]
        public IActionResult Update(int id)
        {
            try
            {
                _logger.LogInformation($"TechnicalConditionsOfRoadsController/Update/{id}", $"Reading a technical condition of road with Id {id}...");

                var result = _technicalConditionsOfRoadsApi.Get(id);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"TechnicalConditionsOfRoadsController/Update/{id}", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var technicalConditionOfRoad = JsonConvert.DeserializeObject<TechnicalConditionOfRoad>(value.ToString());

                _logger.LogInformation($"TechnicalConditionsOfRoadsController/Update/{id}", $"The technical condition of road with Id {id} was successfully read.");

                _logger.LogInformation($"TechnicalConditionsOfRoadsController/Update/{id}", "Reading all roads...");

                result = _roadsApi.Get();
                statusCode = ((ObjectResult)result).StatusCode;
                value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"TechnicalConditionsOfRoadsController/Update/{id}", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var roads = JsonConvert.DeserializeObject<IEnumerable<Road>>(value.ToString());

                _logger.LogInformation($"TechnicalConditionsOfRoadsController/Update/{id}", "All roads have been successfully read.");

                TechnicalConditionOfRoadRoadsViewModel viewModel = new()
                {
                    TechnicalConditionOfRoad = technicalConditionOfRoad,
                    Roads = roads,
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

                _logger.LogInformation("TechnicalConditionsOfRoadsController", "Navigating to the page \"Update Technical Condition Of Road\".");

                return View("Update", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("TechnicalConditionsOfRoadsController", $"Error when navigating to the page \"Update Technical Condition Of Road\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("update/{id}")]
        public IActionResult Update(TechnicalConditionOfRoad technicalConditionOfRoad)
        {
            try
            {
                _logger.LogInformation($"TechnicalConditionsOfRoadsController/Update/{technicalConditionOfRoad.Id}", $"Updating the technical condition of road with Id {technicalConditionOfRoad.Id}...");

                if (technicalConditionOfRoad == null)
                {
                    _logger.LogWarning($"TechnicalConditionsOfRoadsController/Update/{technicalConditionOfRoad.Id}", "Incorrect technical condition of road data provided.");
                    return BadRequest("Incorrect technical condition of road data provided");
                }

                TechnicalConditionOfRoadViewModel technicalConditionOfRoadData = new()
                {
                    Year = technicalConditionOfRoad.Year,
                    Month = technicalConditionOfRoad.Month,
                    TechnicalCondition = technicalConditionOfRoad.TechnicalCondition,
                    RoadId = technicalConditionOfRoad.RoadId,
                };

                var result = _technicalConditionsOfRoadsApi.Put(technicalConditionOfRoad.Id, technicalConditionOfRoadData);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"TechnicalConditionsOfRoadsController/Update/{technicalConditionOfRoad.Id}", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                technicalConditionOfRoad = JsonConvert.DeserializeObject<TechnicalConditionOfRoad>(value.ToString());

                _logger.LogInformation($"TechnicalConditionsOfRoadsController/Update/{technicalConditionOfRoad.Id}", $"The technical condition of road with Id {technicalConditionOfRoad.Id} has been successfully updated.");

                return Read(technicalConditionOfRoad.RoadId, technicalConditionOfRoad.Year);
            }
            catch (Exception ex)
            {
                _logger.LogError($"TechnicalConditionsOfRoadsController/Update/{technicalConditionOfRoad.Id}", $"Error updating the technical condition of road with Id {technicalConditionOfRoad.Id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation($"TechnicalConditionsOfRoadsController/Delete/{id}", $"Deleting a technical condition of road with Id {id}...");

                var result = _technicalConditionsOfRoadsApi.Delete(id);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"TechnicalConditionsOfRoadsController/Delete/{id}", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                _logger.LogInformation($"TechnicalConditionsOfRoadsController/Delete/{id}", $"The technical condition of road with Id {id} has been successfully deleted.");

                return RedirectToAction("Read");
            }
            catch (Exception ex)
            {
                _logger.LogError($"TechnicalConditionsOfRoadsController/Delete/{id}", $"Error when deleting a technical condition of road with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
