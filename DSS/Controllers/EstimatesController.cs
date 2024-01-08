using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using DSS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DSS.Controllers
{
    [Route("estimates")]
    public class EstimatesController : Controller
    {
        private readonly EstimatesApiController _estimatesApi;
        private readonly ApiLogger _logger;

        public EstimatesController(ApplicationContext context, ILogger<EstimatesApiController> logger)
        {
            _estimatesApi = new EstimatesApiController(context, logger);
            _logger = new ApiLogger(logger);
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

                _logger.LogInformation("EstimatesController", "Navigating to the page \"Read All Estimates\".");

                return View("Index", estimates);
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
                _logger.LogInformation("EstimatesController", "Navigating to the page \"Create Estimate\".");
                return View("Create");
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

                _logger.LogInformation("EstimatesController", "Navigating to the page \"Update Estimate\".");

                return View("Update", estimate);
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

                _logger.LogInformation("EstimatesController/Update", $"The estimate with Id {estimate.Id} has been successfully updated.");

                return RedirectToAction("Read");
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesController/Update", $"Error updating the estimate with Id {estimate.Id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation($"EstimatesController/Delete/{id}", $"Reading a estimate with Id {id}...");

                var result = _estimatesApi.Get(id);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"EstimatesController/Delete/{id}", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var estimate = JsonConvert.DeserializeObject<Estimate>(value.ToString());

                _logger.LogInformation($"EstimatesController/Delete/{id}", $"The estimate with Id {id} was successfully read.");

                _logger.LogInformation("EstimatesController", "Navigating to the page \"Delete Estimate\".");

                return View("Delete", estimate);
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesController", $"Error when navigating to the page \"Delete Estimate\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("delete/{id}")]
        public IActionResult Delete(Estimate estimate)
        {
            try
            {
                _logger.LogInformation("EstimatesController/Delete", $"Deleting a estimate with Id {estimate.Id}...");

                if (estimate == null)
                {
                    _logger.LogWarning("EstimatesController/Delete", "Incorrect estimate data provided.");
                    return BadRequest("Incorrect estimate data provided");
                }

                var result = _estimatesApi.Delete(estimate.Id);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"EstimatesController/Delete", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                _logger.LogInformation("EstimatesController/Delete", $"The estimate with Id {estimate.Id} has been successfully deleted.");

                return RedirectToAction("Read");
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesController/Delete", $"Error when deleting a estimate with Id {estimate.Id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
