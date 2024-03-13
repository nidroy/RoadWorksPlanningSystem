using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models.ViewModels;
using DSS.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DSS.Handlers;

namespace DSS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("roadWorksPrograms")]
    public class RoadWorksProgramsController : Controller
    {
        private readonly RoadsApiController _roadsApi;
        private readonly EstimatesApiController _estimatesApi;
        private readonly RoadWorksProgramsApiController _roadWorksProgramsApi;
        private readonly ApiLogger _logger;

        public RoadWorksProgramsController(ApplicationContext context, ILogger<ApiController> logger)
        {
            _roadsApi = new RoadsApiController(context, logger);
            _estimatesApi = new EstimatesApiController(context, logger);
            _roadWorksProgramsApi = new RoadWorksProgramsApiController(context, logger);
            _logger = new ApiLogger(logger);
        }

        [HttpGet("read")]
        public IActionResult Read()
        {
            try
            {
                _logger.LogInformation("RoadWorksProgramsController/Read", "Reading all road works programs...");

                var result = _roadWorksProgramsApi.Get();
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("RoadWorksProgramsController/Read", "Error on the API side of the controller.");
                    return BadRequest(value);
                }

                var roadWorksPrograms = JsonConvert.DeserializeObject<IEnumerable<RoadWorksProgramEstimatesViewModel>>(value.ToString());

                _logger.LogInformation("RoadWorksProgramsController/Read", "All road works programs have been successfully read.");

                _logger.LogInformation("RoadWorksProgramsController/Read", "Reading all roads...");

                result = _roadsApi.Get();
                statusCode = ((ObjectResult)result).StatusCode;
                value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("RoadWorksProgramsController/Read", "Error on the API side of the controller.");
                    return BadRequest(value);
                }

                var roads = JsonConvert.DeserializeObject<IEnumerable<Road>>(value.ToString());

                _logger.LogInformation("RoadWorksProgramsController/Read", "All roads have been successfully read.");

                List<RoadRoadWorksProgramsViewModel> viewModels = new();

                foreach (var road in roads)
                {
                    var roadRoadWorksPrograms = roadWorksPrograms
                        .Where(p => p.RoadId == road.Id)
                        .GroupBy(p => p.Year)
                        .ToDictionary(p => p.Key, p => p.AsEnumerable());

                    RoadRoadWorksProgramsViewModel viewModel = new()
                    {
                        Road = road,
                        RoadWorksPrograms = roadRoadWorksPrograms
                    };

                    viewModels.Add(viewModel);
                }

                _logger.LogInformation("RoadWorksProgramsController", "Navigating to the page \"Read All Road Works Programs By Year\".");

                return View("Index", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadWorksProgramsController", $"Error when navigating to the page \"Read All Road Works Programs By Year\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("read/{roadId}/{year}")]
        public IActionResult Read(int roadId, int year)
        {
            try
            {
                _logger.LogInformation($"RoadWorksProgramsController/Read/{roadId}/{year}", $"Reading the road works program for {year}...");

                var result = _roadWorksProgramsApi.Get();
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"RoadWorksProgramsController/Read/{roadId}/{year}", "Error on the API side of the controller.");
                    return BadRequest(value);
                }

                var roadWorksPrograms = JsonConvert.DeserializeObject<IEnumerable<RoadWorksProgramEstimatesViewModel>>(value.ToString());

                _logger.LogInformation($"RoadWorksProgramsController/Read/{roadId}/{year}", $"The road works program for {year} was successfully read.");

                var roadWorksProgramForYear = roadWorksPrograms
                    .Where(p => p.RoadId == roadId && p.Year == year)
                    .ToList();

                _logger.LogInformation("RoadWorksProgramsController", $"Navigating to the page \"Read Road Works Program For {year}\".");

                return View("Read", roadWorksProgramForYear);
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadWorksProgramsController", $"Error when navigating to the page \"Read Road Works Program For {year}\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            try
            {
                _logger.LogInformation("RoadWorksProgramsController/Create", "Reading all roads...");

                var result = _roadsApi.Get();
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("RoadWorksProgramsController/Create", "Error on the API side of the controller.");
                    return BadRequest(value);
                }

                var roads = JsonConvert.DeserializeObject<IEnumerable<Road>>(value.ToString());

                _logger.LogInformation("RoadWorksProgramsController/Create", "All roads have been successfully read.");

                _logger.LogInformation("RoadWorksProgramsController/Create", "Reading all estimates...");

                result = _estimatesApi.Get();
                statusCode = ((ObjectResult)result).StatusCode;
                value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("RoadWorksProgramsController/Create", "Error on the API side of the controller.");
                    return BadRequest(value);
                }

                var estimates = JsonConvert.DeserializeObject<IEnumerable<Estimate>>(value.ToString());

                _logger.LogInformation("RoadWorksProgramsController/Create", "All estimates have been successfully read.");

                RoadWorksProgramRoadsEstimatesViewModel viewModel = new()
                {
                    RoadWorksProgram = new(),
                    Roads = roads,
                    Estimates = estimates,
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

                _logger.LogInformation("RoadWorksProgramsController", "Navigating to the page \"Create Road Works Program\".");

                return View("Create", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadWorksProgramsController", $"Error when navigating to the page \"Create Road Works Program\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("create")]
        public IActionResult Create(RoadWorksProgram roadWorksProgram, string selectedEstimatesId)
        {
            try
            {
                _logger.LogInformation("RoadWorksProgramsController/Create", "Creating a new road works program...");

                if (roadWorksProgram == null)
                {
                    _logger.LogWarning("RoadWorksProgramsController/Create", "Incorrect road works program data provided.");
                    return BadRequest("Incorrect road works program data provided");
                }

                List<int> estimatesId = selectedEstimatesId.Split(' ').Select(int.Parse).ToList();

                RoadWorksProgramViewModel roadWorksProgramData = new()
                {
                    Year = roadWorksProgram.Year,
                    Month = roadWorksProgram.Month,
                    Cost = roadWorksProgram.Cost,
                    EstimatesId = estimatesId,
                    RoadId = roadWorksProgram.RoadId
                };

                var result = _roadWorksProgramsApi.Post(roadWorksProgramData);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("RoadWorksProgramsController/Create", "Error on the API side of the controller.");
                    return BadRequest(value);
                }

                _logger.LogInformation("RoadWorksProgramsController/Create", $"A new road works program with Id {value} has been successfully created.");

                return RedirectToAction("Read");
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadWorksProgramsController/Create", $"Error when creating a new road works program: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("update/{id}")]
        public IActionResult Update(int id)
        {
            try
            {
                _logger.LogInformation($"RoadWorksProgramsController/Update/{id}", $"Reading a road works program with Id {id}...");

                var result = _roadWorksProgramsApi.Get(id);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"RoadWorksProgramsController/Update/{id}", "Error on the API side of the controller.");
                    return BadRequest(value);
                }

                var roadWorksProgram = JsonConvert.DeserializeObject<RoadWorksProgramEstimatesViewModel>(value.ToString());

                _logger.LogInformation($"RoadWorksProgramsController/Update/{id}", $"The road works program with Id {id} was successfully read.");

                _logger.LogInformation($"RoadWorksProgramsController/Update/{id}", "Reading all roads...");

                result = _roadsApi.Get();
                statusCode = ((ObjectResult)result).StatusCode;
                value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"RoadWorksProgramsController/Update/{id}", "Error on the API side of the controller.");
                    return BadRequest(value);
                }

                var roads = JsonConvert.DeserializeObject<IEnumerable<Road>>(value.ToString());

                _logger.LogInformation($"RoadWorksProgramsController/Update/{id}", "All roads have been successfully read.");

                _logger.LogInformation($"RoadWorksProgramsController/Update/{id}", "Reading all estimates...");

                result = _estimatesApi.Get();
                statusCode = ((ObjectResult)result).StatusCode;
                value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"RoadWorksProgramsController/Update/{id}", "Error on the API side of the controller.");
                    return BadRequest(value);
                }

                var estimates = JsonConvert.DeserializeObject<IEnumerable<Estimate>>(value.ToString());

                _logger.LogInformation($"RoadWorksProgramsController/Update/{id}", "All estimates have been successfully read.");

                RoadWorksProgramRoadsEstimatesViewModel viewModel = new()
                {
                    RoadWorksProgram = new()
                    {
                        Id = roadWorksProgram.Id,
                        Year = roadWorksProgram.Year,
                        Month = roadWorksProgram.Month,
                        Cost = roadWorksProgram.Cost,
                        RoadId = roadWorksProgram.Id,
                        Road = roadWorksProgram.Road
                    },
                    Roads = roads,
                    Estimates = estimates,
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

                _logger.LogInformation("RoadWorksProgramsController", "Navigating to the page \"Update Road Works Program\".");

                return View("Update", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadWorksProgramsController", $"Error when navigating to the page \"Update Road Works Program\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("update/{id}")]
        public IActionResult Update(RoadWorksProgram roadWorksProgram, string selectedEstimatesId)
        {
            try
            {
                _logger.LogInformation($"RoadWorksProgramsController/Update/{roadWorksProgram.Id}", $"Updating the road works program with Id {roadWorksProgram.Id}...");

                if (roadWorksProgram == null)
                {
                    _logger.LogWarning($"RoadWorksProgramsController/Update/{roadWorksProgram.Id}", "Incorrect road works program data provided.");
                    return BadRequest("Incorrect road works program data provided");
                }

                List<int> estimatesId = selectedEstimatesId.Split(' ').Select(int.Parse).ToList();

                RoadWorksProgramViewModel roadWorksProgramData = new()
                {
                    Year = roadWorksProgram.Year,
                    Month = roadWorksProgram.Month,
                    Cost = roadWorksProgram.Cost,
                    EstimatesId = estimatesId,
                    RoadId = roadWorksProgram.RoadId
                };

                var result = _roadWorksProgramsApi.Put(roadWorksProgram.Id, roadWorksProgramData);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"RoadWorksProgramsController/Update/{roadWorksProgram.Id}", "Error on the API side of the controller.");
                    return BadRequest(value);
                }

                var resultRoadWorksProgram = JsonConvert.DeserializeObject<RoadWorksProgramEstimatesViewModel>(value.ToString());

                _logger.LogInformation($"RoadWorksProgramsController/Update/{roadWorksProgram.Id}", $"The road works program with Id {roadWorksProgram.Id} has been successfully updated.");

                return Read(resultRoadWorksProgram.RoadId, resultRoadWorksProgram.Year);
            }
            catch (Exception ex)
            {
                _logger.LogError($"RoadWorksProgramsController/Update/{roadWorksProgram.Id}", $"Error updating the road works program with Id {roadWorksProgram.Id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation($"RoadWorksProgramsController/Delete/{id}", $"Deleting a road works program with Id {id}...");

                var result = _roadWorksProgramsApi.Delete(id);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"RoadWorksProgramsController/Delete/{id}", "Error on the API side of the controller.");
                    return BadRequest(value);
                }

                _logger.LogInformation($"RoadWorksProgramsController/Delete/{id}", $"The road works program with Id {id} has been successfully deleted.");

                return RedirectToAction("Read");
            }
            catch (Exception ex)
            {
                _logger.LogError($"RoadWorksProgramsController/Delete/{id}", $"Error when deleting a road works program with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("export")]
        public IActionResult Export(string roadWorksPrograms)
        {
            try
            {
                _logger.LogInformation("RoadWorksProgramsController/Export", "Exporting a road works program...");

                string folderPath = @"Data\RoadWorksPrograms";

                var roadWorksProgramsData = JsonConvert.DeserializeObject<List<RoadWorksProgramEstimatesViewModel>>(roadWorksPrograms);

                if (roadWorksProgramsData == null)
                {
                    _logger.LogWarning("RoadWorksProgramsController/Export", "Incorrect road works program data provided.");
                    return BadRequest("Incorrect road works program data provided");
                }

                string fileName = $"{roadWorksProgramsData.First().Road.Number} {roadWorksProgramsData.First().Year} год.xlsx";
                string filePath = Path.Combine(folderPath, fileName);

                bool isWrittenRoadWorksProgramToExcelFile = FileHandler.WriteRoadWorksProgramToExcelFile(filePath, roadWorksProgramsData);

                if (!isWrittenRoadWorksProgramToExcelFile)
                {
                    _logger.LogError("RoadWorksProgramsController/Export", "Error when writing a road works program to excel file.");
                    return StatusCode(500, "Internal server error");
                }

                _logger.LogInformation("RoadWorksProgramsController/Export", "The road works program has been successfully exported.");

                return Read(roadWorksProgramsData.First().RoadId, roadWorksProgramsData.First().Year);
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadWorksProgramsController/Export", $"Error when exporting a road works program: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
