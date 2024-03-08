﻿using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models.ViewModels;
using DSS.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

                return RedirectToAction("Read");
            }
            catch (Exception ex)
            {
                _logger.LogError("TechnicalConditionsOfRoadsController/Create", $"Error when creating a new technical condition of road: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}