using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using DSS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DSS.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("roads")]
    public class RoadsController : Controller
    {
        private readonly RoadsApiController _roadsApi;
        private readonly ApiLogger _logger;

        public RoadsController(ApplicationContext context, ILogger<ApiController> logger)
        {
            _roadsApi = new RoadsApiController(context, logger);
            _logger = new ApiLogger(logger);
        }

        [HttpGet("read")]
        public IActionResult Read()
        {
            try
            {
                _logger.LogInformation("RoadsController/Read", "Reading all roads...");

                var result = _roadsApi.Get();
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("RoadsController/Read", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var roads = JsonConvert.DeserializeObject<IEnumerable<Road>>(value.ToString());

                _logger.LogInformation("RoadsController/Read", "All roads have been successfully read.");

                _logger.LogInformation("RoadsController", "Navigating to the page \"Read All Roads\".");

                return View("Index", roads);
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadsController", $"Error when navigating to the page \"Read All Roads\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("read/{id}")]
        public IActionResult Read(int id)
        {
            try
            {
                _logger.LogInformation($"RoadsController/Read/{id}", $"Reading a road with Id {id}...");

                var result = _roadsApi.Get(id);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"RoadsController/Read/{id}", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var road = JsonConvert.DeserializeObject<Road>(value.ToString());

                _logger.LogInformation($"RoadsController/Read/{id}", $"The road with Id {id} was successfully read.");

                _logger.LogInformation("RoadsController", "Navigating to the page \"Read Road By Id\".");

                return View("Read", road);
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadsController", $"Error when navigating to the page \"Read Road By Id\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            try
            {
                Road road = new();
                _logger.LogInformation("RoadsController", "Navigating to the page \"Create Road\".");
                return View("Create", road);
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadsController", $"Error when navigating to the page \"Create Road\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("create")]
        public IActionResult Create(Road road)
        {
            try
            {
                _logger.LogInformation("RoadsController/Create", "Creating a new road...");

                if (road == null)
                {
                    _logger.LogWarning("RoadsController/Create", "Incorrect road data provided.");
                    return BadRequest("Incorrect road data provided");
                }

                RoadViewModel roadData = new()
                {
                    Number = road.Number,
                    Priority = road.Priority,
                    LinkToPassport = road.LinkToPassport
                };

                var result = _roadsApi.Post(roadData);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"RoadsController/Create", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                _logger.LogInformation("RoadsController/Create", $"A new road with Id {value} has been successfully created.");

                return RedirectToAction("Read");
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadsController/Create", $"Error when creating a new road: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("update/{id}")]
        public IActionResult Update(int id)
        {
            try
            {
                _logger.LogInformation($"RoadsController/Update/{id}", $"Reading a road with Id {id}...");

                var result = _roadsApi.Get(id);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"RoadsController/Update/{id}", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var road = JsonConvert.DeserializeObject<Road>(value.ToString());

                _logger.LogInformation($"RoadsController/Update/{id}", $"The road with Id {id} was successfully read.");

                _logger.LogInformation("RoadsController", "Navigating to the page \"Update Road\".");

                return View("Update", road);
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadsController", $"Error when navigating to the page \"Update Road\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("update/{id}")]
        public IActionResult Update(Road road)
        {
            try
            {
                _logger.LogInformation("RoadsController/Update", $"Updating the road with Id {road.Id}...");

                if (road == null)
                {
                    _logger.LogWarning("RoadsController/Update", "Incorrect road data provided.");
                    return BadRequest("Incorrect road data provided");
                }

                RoadViewModel roadData = new()
                {
                    Number = road.Number,
                    Priority = road.Priority,
                    LinkToPassport = road.LinkToPassport
                };

                var result = _roadsApi.Put(road.Id, roadData);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"RoadsController/Update", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                _logger.LogInformation("RoadsController/Update", $"The road with Id {road.Id} has been successfully updated.");

                return RedirectToAction("Read");
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadsController/Update", $"Error updating the road with Id {road.Id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation("RoadsController/Delete", $"Deleting a road with Id {id}...");

                var result = _roadsApi.Delete(id);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"RoadsController/Delete", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                _logger.LogInformation("RoadsController/Delete", $"The road with Id {id} has been successfully deleted.");

                return RedirectToAction("Read");
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadsController/Delete", $"Error when deleting a road with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
