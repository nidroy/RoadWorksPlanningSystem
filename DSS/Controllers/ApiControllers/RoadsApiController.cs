using DSS.Loggers;
using DSS.Models;
using DSS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSS.Controllers.ApiControllers
{
    [ApiController]
    [Route("api/roads")]
    public class RoadsApiController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ApiLogger _logger;

        public RoadsApiController(ApplicationContext context, ILogger<RoadsApiController> logger)
        {
            _context = context;
            _logger = new ApiLogger(logger);
        }

        /// <summary>
        /// Получаем данные всех дорог
        /// </summary>
        /// <returns>Данные всех дорог</returns>
        [HttpGet("get")]
        public IActionResult Get()
        {
            try
            {
                _logger.LogInformation("RoadsApiController/Get", "Getting all the roads...");

                // Получаем все дороги из контекста данных
                List<Road> roads = _context.Roads.ToList();

                // Преобразуем список дорог в JSON массив
                JArray result = new JArray(
                    roads.Select(road => new JObject(
                        new JProperty("Id", road.Id),
                        new JProperty("Number", road.Number),
                        new JProperty("Priority", road.Priority),
                        new JProperty("LinkToPassport", road.LinkToPassport)
                    ))
                );

                _logger.LogInformation("RoadsApiController/Get", "All roads have been successfully received.");

                // Возвращаем успешный результат с JSON массивом дорог
                return Ok(result.ToString());
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("RoadsApiController/Get", $"Error in getting all roads: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получаем данные дороги по id
        /// </summary>
        /// <param name="id">Идентификатор дороги</param>
        /// <returns>Данные дороги</returns>
        [HttpGet("get/{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                _logger.LogInformation($"RoadsApiController/Get/{id}", $"Getting a road with Id {id}...");

                // Ищем дорогу по указанному ID в контексте данных
                Road road = _context.Roads.FirstOrDefault(r => r.Id == id);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning($"RoadsApiController/Get/{id}", $"The road with Id {id} was not found.");
                    return NotFound($"The road with Id {id} was not found");
                }

                // Преобразуем найденную дорогу в JSON объект
                string result = JsonConvert.SerializeObject(road, Formatting.Indented);

                _logger.LogInformation($"RoadsApiController/Get/{id}", $"The road with Id {id} was successfully received.");

                // Возвращаем успешный результат с JSON объектом дороги
                return Ok(result);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError($"RoadsApiController/Get/{id}", $"Error when getting a road with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Создаем новую дорогу
        /// </summary>
        /// <param name="roadData">Данные новой дороги</param>
        /// <returns>Идентификатор новой дороги</returns>
        [HttpPost("post")]
        public IActionResult Post([FromBody] RoadViewModel roadData)
        {
            try
            {
                _logger.LogInformation("RoadsApiController/Post", $"Creating a new road...");

                // Проверяем входные данные на null
                if (roadData == null)
                {
                    _logger.LogWarning("RoadsApiController/Post", "Incorrect road data provided.");
                    return BadRequest("Incorrect road data provided");
                }

                // Создаем новый объект Road на основе входных данных
                Road road = new()
                {
                    Number = roadData.Number,
                    Priority = roadData.Priority,
                    LinkToPassport = roadData.LinkToPassport
                };

                // Добавляем новую дорогу в контекст данных
                _context.Roads.Add(road);
                _context.SaveChanges();

                _logger.LogInformation("RoadsApiController/Post", $"A new road with Id {road.Id} has been successfully created.");

                // Возвращаем успешный результат с Id новой дороги
                return Ok(road.Id);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("RoadsApiController/Post", $"Error when creating a new road: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Обновляем данные дороги
        /// </summary>
        /// <param name="id">Идентификатор дороги</param>
        /// <param name="roadData">Новые данные дороги</param>
        /// <returns>Данные обновленной дороги</returns>
        [HttpPut("put/{id}")]
        public IActionResult Put(int id, [FromBody] RoadViewModel roadData)
        {
            try
            {
                _logger.LogInformation($"RoadsApiController/Put/{id}", $"Updating the road with Id {id}...");

                // Ищем дорогу по указанному ID в контексте данных
                Road road = _context.Roads.FirstOrDefault(r => r.Id == id);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning($"RoadsApiController/Put/{id}", $"The road with Id {id} was not found.");
                    return NotFound($"The road with Id {id} was not found");
                }

                // Проверяем входные данные на null
                if (roadData == null)
                {
                    _logger.LogWarning($"RoadsApiController/Put/{id}", "Incorrect road data provided.");
                    return BadRequest("Incorrect road data provided");
                }

                // Обновляем свойства дороги на основе входных данных
                road.Number = roadData.Number;
                road.Priority = roadData.Priority;
                road.LinkToPassport = roadData.LinkToPassport;

                // Сохраняем изменения в контексте данных
                _context.Roads.Update(road);
                _context.SaveChanges();

                _logger.LogInformation($"RoadsApiController/Put/{id}", $"The road with Id {id} has been successfully updated.");

                // Преобразуем обновленную дорогу в JSON объект
                string result = JsonConvert.SerializeObject(road, Formatting.Indented);

                // Возвращаем успешный результат с JSON объектом обновленной дороги
                return Ok(result);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError($"RoadsApiController/Put/{id}", $"Error updating the road with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удаляем дорогу
        /// </summary>
        /// <param name="id">Идентификатор дороги</param>
        /// <returns>Количество оставшихся дорог</returns>
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation($"RoadsApiController/Delete/{id}", $"Deleting a road with Id {id}...");

                // Ищем дорогу по указанному ID в контексте данных
                Road road = _context.Roads.FirstOrDefault(r => r.Id == id);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning($"RoadsApiController/Delete/{id}", $"The road with Id {id} was not found.");
                    return NotFound($"The road with Id {id} was not found");
                }

                // Удаляем дорогу из контекста данных
                _context.Roads.Remove(road);
                _context.SaveChanges();

                _logger.LogInformation($"RoadsApiController/Delete/{id}", $"The road with Id {id} has been successfully deleted.");

                // Получаем все оставшиеся дороги из контекста данных
                List<Road> roads = _context.Roads.ToList();

                // Возвращаем успешный результат с количеством оставшихся дорог
                return Ok(roads.Count);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError($"RoadsApiController/Delete/{id}", $"Error when deleting a road with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}