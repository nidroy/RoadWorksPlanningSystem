using DSS.Models;
using DSS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSS.Controllers.ApiControllers
{
    [ApiController]
    [Route("api/estimates")]
    public class EstimatesApiController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<EstimatesApiController> _logger;

        public EstimatesApiController(ApplicationContext context, ILogger<EstimatesApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Получаем данные всех смет
        /// </summary>
        /// <returns>Данные всех смет</returns>
        [HttpGet("get")]
        public IActionResult Get()
        {
            try
            {
                _logger.LogInformation("EstimatesApiController", "Getting all the estimates...");

                // Получаем все сметы из контекста данных
                List<Estimate> estimates = _context.Estimates.ToList();

                // Преобразуем список смет в JSON массив
                JArray result = new JArray(
                    estimates.Select(estimate => new JObject(
                        new JProperty("Id", estimate.Id),
                        new JProperty("Name", estimate.Name),
                        new JProperty("LevelOfWork", estimate.LevelOfWork),
                        new JProperty("Cost", estimate.Cost),
                        new JProperty("Link", estimate.Link),
                        new JProperty("RoadId", estimate.RoadId)
                    ))
                );

                _logger.LogInformation("EstimatesApiController", "All estimates have been successfully received.");

                // Возвращаем успешный результат с JSON массивом смет
                return Ok(result.ToString());
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("EstimatesApiController", $"Error in getting all estimates: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получаем данные сметы по id
        /// </summary>
        /// <param name="id">Идентификатор сметы</param>
        /// <returns>Данные сметы</returns>
        [HttpGet("get/{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                _logger.LogInformation("EstimatesApiController", $"Getting a estimate with Id {id}...");

                // Ищем смету по указанному ID в контексте данных
                Estimate estimate = _context.Estimates.FirstOrDefault(e => e.Id == id);

                if (estimate == null)
                {
                    // Возвращаем 404 Not Found, если смета не найдена
                    _logger.LogWarning("EstimatesApiController", $"The estimate with Id {id} was not found.");
                    return NotFound(null);
                }

                // Ищем дорогу по ID дороги в сметe
                Road road = _context.Roads.FirstOrDefault(r => r.Id == estimate.RoadId);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning("EstimatesApiController", $"The road with Id {estimate.RoadId} was not found.");
                    return NotFound(null);
                }

                // Преобразуем найденную смету в JSON объект
                string result = JsonConvert.SerializeObject(estimate, Formatting.Indented);

                _logger.LogInformation("EstimatesApiController", $"The estimate with Id {id} was successfully received.");

                // Возвращаем успешный результат с JSON объектом сметы
                return Ok(result);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("EstimatesApiController", $"Error when getting a estimate with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Создаем новую смету
        /// </summary>
        /// <param name="estimateData">Данные новой сметы</param>
        /// <returns>Идентификатор новой сметы</returns>
        [HttpPost("post")]
        public IActionResult Post([FromBody] EstimateViewModel estimateData)
        {
            try
            {
                _logger.LogInformation("EstimatesApiController", $"Creating a new estimate...");

                // Проверяем входные данные на null
                if (estimateData == null)
                {
                    _logger.LogWarning("EstimatesApiController", "Incorrect estimate data provided.");
                    return BadRequest(null);
                }

                // Ищем дорогу по ID дороги во входных данных
                Road road = _context.Roads.FirstOrDefault(r => r.Id == estimateData.RoadId);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning("EstimatesApiController", $"The road with Id {estimateData.RoadId} was not found.");
                    return NotFound(null);
                }

                // Создаем новый объект Estimate на основе входных данных
                Estimate estimate = new()
                {
                    Name = estimateData.Name,
                    LevelOfWork = estimateData.LevelOfWork,
                    Cost = estimateData.Cost,
                    Link = estimateData.Link,
                    RoadId = estimateData.RoadId,
                };

                // Добавляем новую смету в контекст данных
                _context.Estimates.Add(estimate);
                _context.SaveChanges();

                _logger.LogInformation("EstimatesApiController", $"A new estimate with Id {estimate.Id} has been successfully created.");

                // Возвращаем успешный результат с Id новой сметы
                return Ok(estimate.Id);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("EstimatesApiController", $"Error when creating a new estimate: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Обновляем данные сметы
        /// </summary>
        /// <param name="id">Идентификатор сметы</param>
        /// <param name="estimateData">Новые данные сметы</param>
        /// <returns>Данные обновленной сметы</returns>
        [HttpPut("put/{id}")]
        public IActionResult Put(int id, [FromBody] EstimateViewModel estimateData)
        {
            try
            {
                _logger.LogInformation("EstimatesApiController", $"Updating the estimate with Id {id}...");

                // Проверяем входные данные на null
                if (estimateData == null)
                {
                    _logger.LogWarning("EstimatesApiController", "Incorrect estimate data provided.");
                    return BadRequest(null);
                }

                // Ищем смету по указанному ID в контексте данных
                Estimate estimate = _context.Estimates.FirstOrDefault(e => e.Id == id);

                if (estimate == null)
                {
                    // Возвращаем 404 Not Found, если смета не найдена
                    _logger.LogWarning("EstimatesApiController", $"The estimate with Id {id} was not found.");
                    return NotFound(null);
                }

                // Ищем дорогу по ID дороги в сметe
                Road road = _context.Roads.FirstOrDefault(r => r.Id == estimate.RoadId);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning("EstimatesApiController", $"The road with Id {estimate.RoadId} was not found.");
                    return NotFound(null);
                }

                // Обновляем свойства сметы на основе входных данных
                estimate.Name = estimateData.Name;
                estimate.LevelOfWork = estimateData.LevelOfWork;
                estimate.Cost = estimateData.Cost;
                estimate.Link = estimateData.Link;
                estimate.RoadId = estimateData.RoadId;

                // Сохраняем изменения в контексте данных
                _context.Estimates.Update(estimate);
                _context.SaveChanges();

                _logger.LogInformation("EstimatesApiController", $"The estimate with Id {id} has been successfully updated.");

                // Преобразуем обновленную смету в JSON объект
                string result = JsonConvert.SerializeObject(estimate, Formatting.Indented);

                // Возвращаем успешный результат с JSON объектом обновленной сметы
                return Ok(result);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("EstimatesApiController", $"Error updating the estimate with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удаляем смету
        /// </summary>
        /// <param name="id">Идентификатор сметы</param>
        /// <returns>Данные всех оставшихся смет</returns>
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation("EstimatesApiController", $"Deleting a estimate with Id {id}...");

                // Ищем смету по указанному ID в контексте данных
                Estimate estimate = _context.Estimates.FirstOrDefault(e => e.Id == id);

                if (estimate == null)
                {
                    // Возвращаем 404 Not Found, если смета не найдена
                    _logger.LogWarning("EstimatesApiController", $"The estimate with Id {id} was not found.");
                    return NotFound(null);
                }

                // Ищем дорогу по ID дороги в сметe
                Road road = _context.Roads.FirstOrDefault(r => r.Id == estimate.RoadId);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning("EstimatesApiController", $"The road with Id {estimate.RoadId} was not found.");
                    return NotFound(null);
                }

                // Удаляем смету из контекста данных
                _context.Estimates.Remove(estimate);
                _context.SaveChanges();

                _logger.LogInformation("EstimatesApiController", $"The estimate with Id {id} has been successfully deleted.");

                // Получаем все оставшиеся сметы из контекста данных
                List<Estimate> estimates = _context.Estimates.ToList();

                // Преобразуем список смет в JSON массив
                JArray result = new JArray(
                    estimates.Select(estimate => new JObject(
                        new JProperty("Id", estimate.Id),
                        new JProperty("Name", estimate.Name),
                        new JProperty("LevelOfWork", estimate.LevelOfWork),
                        new JProperty("Cost", estimate.Cost),
                        new JProperty("Link", estimate.Link),
                        new JProperty("RoadId", estimate.RoadId)
                    ))
                );

                // Возвращаем успешный результат с JSON массивом оставшихся смет
                return Ok(result.ToString());
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("EstimatesApiController", $"Error when deleting a estimate with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
