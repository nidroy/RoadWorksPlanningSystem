using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DSS.Controllers
{
    public class RoadsController : Controller
    {
        private readonly RoadsApiController _roadsApi;
        private readonly ApiLogger _logger;

        public RoadsController(ApplicationContext context, ILogger<RoadsApiController> logger)
        {
            _roadsApi = new RoadsApiController(context, logger);
            _logger = new ApiLogger(logger);
        }

        public IActionResult Index()
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

                return View(roads);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("RoadsController/Read", $"Error when reading all roads: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        public IActionResult ReadRoad(int id)
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
                
                _logger.LogInformation("RoadsController", "Navigating to the page \"ReadRoad\".");

                return View("ReadRoad", road);
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadsController", $"Error when navigating to the page \"ReadRoad\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        public IActionResult CreateRoad()
        {
            try
            {
                _logger.LogInformation("RoadsController", "Navigating to the page \"CreateRoad\".");
                return View("CreateRoad");
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadsController", $"Error when navigating to the page \"CreateRoad\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        public IActionResult UpdateRoad(int id)
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

                _logger.LogInformation("RoadsController", "Navigating to the page \"UpdateRoad\".");

                return View("UpdateRoad", road);
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadsController", $"Error when navigating to the page \"UpdateRoad\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        public IActionResult DeleteRoad(int id)
        {
            try
            {
                _logger.LogInformation($"RoadsController/Delete/{id}", $"Reading a road with Id {id}...");

                var result = _roadsApi.Get(id);
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning($"RoadsController/Delete/{id}", "Error on the API side of the controller");
                    return BadRequest(value);
                }

                var road = JsonConvert.DeserializeObject<Road>(value.ToString());

                _logger.LogInformation($"RoadsController/Delete/{id}", $"The road with Id {id} was successfully read.");

                _logger.LogInformation("RoadsController", "Navigating to the page \"DeleteRoad\".");

                return View("DeleteRoad", road);
            }
            catch (Exception ex)
            {
                _logger.LogError("RoadsController", $"Error when navigating to the page \"DeleteRoad\": {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
