using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using DSS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DSS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("estimates")]
    public class EstimatesController : Controller
    {
        private readonly EstimatesApiController _estimatesApi;
        private readonly RoadsApiController _roadsApi;
        private readonly ApiLogger _logger;

        public EstimatesController(ApplicationContext context, ILogger<EstimatesApiController> estimatesApiLogger, ILogger<RoadsApiController> roadsApiLogger)
        {
            _estimatesApi = new EstimatesApiController(context, estimatesApiLogger);
            _roadsApi = new RoadsApiController(context, roadsApiLogger);
            _logger = new ApiLogger(estimatesApiLogger);
        }

        [HttpGet("read")]
        public IActionResult Read()
        {
            try
            {
                _logger.LogInformation("EstimatesController/Read", "Reading all estimates...");

                var result = _estimatesApi.Get();
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("EstimatesController/Read", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var estimates = JsonConvert.DeserializeObject<IEnumerable<Estimate>>(value.ToString());

                _logger.LogInformation("EstimatesController/Read", "All estimates have been successfully read.");

                _logger.LogInformation($"EstimatesController/Read", "Reading all roads...");

                result = _roadsApi.Get();
                statusCode = ((ObjectResult)result).StatusCode;
                value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"EstimatesController/Read", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var roads = JsonConvert.DeserializeObject<IEnumerable<Road>>(value.ToString());

                _logger.LogInformation($"EstimatesController/Read", "All roads have been successfully read.");

                List<RoadEstimatesViewModel> viewModels = new();

                foreach (var road in roads)
                {
                    var roadEstimates = estimates.Where(e => e.RoadId == road.Id);

                    RoadEstimatesViewModel viewModel = new()
                    {
                        Road = road,
                        Estimates = roadEstimates
                    };

                    viewModels.Add(viewModel);
                }

                _logger.LogInformation("EstimatesController", "Navigating to the page \"Read All Estimates\".");

                return View("Index", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesController", $"Error when navigating to the page \"Read All Estimates\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("read/{id}")]
        public IActionResult Read(int id)
        {
            try
            {
                _logger.LogInformation($"EstimatesController/Read/{id}", $"Reading a estimate with Id {id}...");

                var result = _estimatesApi.Get(id);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"EstimatesController/Read/{id}", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var estimate = JsonConvert.DeserializeObject<Estimate>(value.ToString());

                _logger.LogInformation($"EstimatesController/Read/{id}", $"The estimate with Id {id} was successfully read.");

                _logger.LogInformation("EstimatesController", "Navigating to the page \"Read Estimate By Id\".");

                return View("Read", estimate);
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesController", $"Error when navigating to the page \"Read Estimate By Id\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            try
            {
                _logger.LogInformation("EstimatesController/Create", "Reading all roads...");

                var result = _roadsApi.Get();
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("EstimatesController/Create", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var roads = JsonConvert.DeserializeObject<IEnumerable<Road>>(value.ToString());

                _logger.LogInformation("EstimatesController/Create", "All roads have been successfully read.");

                EstimateRoadsViewModel viewModel = new EstimateRoadsViewModel
                {
                    Estimate = new(),
                    Roads = roads
                };

                _logger.LogInformation("EstimatesController", "Navigating to the page \"Create Estimate\".");

                return View("Create", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesController", $"Error when navigating to the page \"Create Estimate\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("create")]
        public IActionResult Create(Estimate estimate)
        {
            try
            {
                _logger.LogInformation("EstimatesController/Create", "Creating a new estimate...");

                if (estimate == null)
                {
                    _logger.LogWarning("EstimatesController/Create", "Incorrect estimate data provided.");
                    return BadRequest("Incorrect estimate data provided");
                }

                EstimateViewModel estimateData = new()
                {
                    Name = estimate.Name,
                    LevelOfWorks = estimate.LevelOfWorks,
                    Cost = estimate.Cost,
                    Link = estimate.Link,
                    RoadId = estimate.RoadId
                };

                var result = _estimatesApi.Post(estimateData);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"EstimatesController/Create", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                _logger.LogInformation("EstimatesController/Create", $"A new estimate with Id {value} has been successfully created.");

                return RedirectToAction("Read");
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesController/Create", $"Error when creating a new estimate: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("update/{id}")]
        public IActionResult Update(int id)
        {
            try
            {
                _logger.LogInformation($"EstimatesController/Update/{id}", $"Reading a estimate with Id {id}...");

                var result = _estimatesApi.Get(id);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"EstimatesController/Update/{id}", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var estimate = JsonConvert.DeserializeObject<Estimate>(value.ToString());

                _logger.LogInformation($"EstimatesController/Update/{id}", $"The estimate with Id {id} was successfully read.");

                _logger.LogInformation($"EstimatesController/Update/{id}", "Reading all roads...");

                result = _roadsApi.Get();
                statusCode = ((ObjectResult)result).StatusCode;
                value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"EstimatesController/Update/{id}", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var roads = JsonConvert.DeserializeObject<IEnumerable<Road>>(value.ToString());

                _logger.LogInformation($"EstimatesController/Update/{id}", "All roads have been successfully read.");

                EstimateRoadsViewModel viewModel = new EstimateRoadsViewModel
                {
                    Estimate = estimate,
                    Roads = roads
                };

                _logger.LogInformation("EstimatesController", "Navigating to the page \"Update Estimate\".");

                return View("Update", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesController", $"Error when navigating to the page \"Update Estimate\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("update/{id}")]
        public IActionResult Update(Estimate estimate)
        {
            try
            {
                _logger.LogInformation("EstimatesController/Update", $"Updating the estimate with Id {estimate.Id}...");

                if (estimate == null)
                {
                    _logger.LogWarning("EstimatesController/Update", "Incorrect estimate data provided.");
                    return BadRequest("Incorrect estimate data provided");
                }

                EstimateViewModel estimateData = new()
                {
                    Name = estimate.Name,
                    LevelOfWorks = estimate.LevelOfWorks,
                    Cost = estimate.Cost,
                    Link = estimate.Link,
                    RoadId = estimate.RoadId
                };

                var result = _estimatesApi.Put(estimate.Id, estimateData);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"EstimatesController/Update", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                estimate = JsonConvert.DeserializeObject<Estimate>(value.ToString());

                _logger.LogInformation("EstimatesController/Update", $"The estimate with Id {estimate.Id} has been successfully updated.");

                return View("Read", estimate);
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesController/Update", $"Error updating the estimate with Id {estimate.Id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation("EstimatesController/Delete", $"Deleting a estimate with Id {id}...");

                var result = _estimatesApi.Delete(id);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"EstimatesController/Delete", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                _logger.LogInformation("EstimatesController/Delete", $"The estimate with Id {id} has been successfully deleted.");

                return RedirectToAction("Read");
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesController/Delete", $"Error when deleting a estimate with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
