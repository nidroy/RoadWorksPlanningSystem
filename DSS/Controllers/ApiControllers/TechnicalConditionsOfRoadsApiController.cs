using DSS.Models;
using DSS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSS.Controllers.ApiControllers
{
    [ApiController]
    [Route("api/technicalConditionsOfRoads")]
    public class TechnicalConditionsOfRoadsApiController : ApiController
    {
        public TechnicalConditionsOfRoadsApiController(ApplicationContext context, ILogger<ApiController> logger) : base(context, logger)
        {
        }

        /// <summary>
        /// Получаем данные всех технических состояний дорог
        /// </summary>
        /// <returns>Данные всех технических состояний дорог</returns>
        [HttpGet("get")]
        public IActionResult Get()
        {
            try
            {
                _logger.LogInformation("TechnicalConditionsOfRoadsApiController/Get", "Getting all the technical conditions of roads...");

                // Получаем все технические состояния дорог из контекста данных
                List<TechnicalConditionOfRoad> technicalConditionsOfRoads = _context.TechnicalConditionsOfRoads.ToList();

                // Преобразуем список технических состояний дорог в JSON массив
                JArray result = new JArray();

                foreach (var technicalConditionOfRoad in technicalConditionsOfRoads)
                {
                    // Ищем дорогу по ID дороги в техническом состоянии дороги
                    Road? road = _context.Roads.FirstOrDefault(r => r.Id == technicalConditionOfRoad.RoadId);

                    if (road == null)
                    {
                        // Возвращаем 404 Not Found, если дорога не найдена
                        _logger.LogWarning("TechnicalConditionsOfRoadsApiController/Get", $"The road with Id {technicalConditionOfRoad.RoadId} was not found.");
                        return NotFound($"The road with Id {technicalConditionOfRoad.RoadId} was not found");
                    }

                    result.Add(new JObject
                    {
                        ["Id"] = technicalConditionOfRoad.Id,
                        ["Year"] = technicalConditionOfRoad.Year,
                        ["Month"] = technicalConditionOfRoad.Month,
                        ["TechnicalCondition"] = technicalConditionOfRoad.TechnicalCondition,
                        ["RoadId"] = technicalConditionOfRoad.RoadId,
                        ["Road"] = new JObject
                        {
                            ["Id"] = road.Id,
                            ["Number"] = road.Number,
                            ["Priority"] = road.Priority,
                            ["LinkToPassport"] = road.LinkToPassport
                        }
                    });
                }

                _logger.LogInformation("TechnicalConditionsOfRoadsApiController/Get", "All technical conditions of roads have been successfully received.");

                // Возвращаем успешный результат с JSON массивом технических состояний дорог
                return Ok(result.ToString());
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("TechnicalConditionsOfRoadsApiController/Get", $"Error in getting all technical conditions of roads: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получаем данные технического состояния дороги по id
        /// </summary>
        /// <param name="id">Идентификатор технического состояния дороги</param>
        /// <returns>Данные технического состояния дороги</returns>
        [HttpGet("get/{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                _logger.LogInformation($"TechnicalConditionsOfRoadsApiController/Get/{id}", $"Getting a technical condition of road with Id {id}...");

                // Ищем техническое состояние дороги по указанному ID в контексте данных
                TechnicalConditionOfRoad? technicalConditionOfRoad = _context.TechnicalConditionsOfRoads.FirstOrDefault(tc => tc.Id == id);

                if (technicalConditionOfRoad == null)
                {
                    // Возвращаем 404 Not Found, если техническое состояние дороги не найдено
                    _logger.LogWarning($"TechnicalConditionsOfRoadsApiController/Get/{id}", $"The technical condition of road with Id {id} was not found.");
                    return NotFound($"The technical condition of road with Id {id} was not found");
                }

                // Ищем дорогу по ID дороги в техническом состоянии дороги
                Road? road = _context.Roads.FirstOrDefault(r => r.Id == technicalConditionOfRoad.RoadId);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning($"TechnicalConditionsOfRoadsApiController/Get/{id}", $"The road with Id {technicalConditionOfRoad.RoadId} was not found.");
                    return NotFound($"The road with Id {technicalConditionOfRoad.RoadId} was not found");
                }

                // Преобразуем найденное техническое состояние дороги в JSON объект
                string result = JsonConvert.SerializeObject(technicalConditionOfRoad, Formatting.Indented);

                _logger.LogInformation($"TechnicalConditionsOfRoadsApiController/Get/{id}", $"The technical condition of road with Id {id} was successfully received.");

                // Возвращаем успешный результат с JSON объектом технического состояния дороги
                return Ok(result);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError($"TechnicalConditionsOfRoadsApiController/Get/{id}", $"Error when getting a technical condition of road with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Создаем новое техническое состояние дороги
        /// </summary>
        /// <param name="technicalConditionOfRoadData">Данные нового технического состояния дороги</param>
        /// <returns>Идентификатор нового технического состояния дороги</returns>
        [HttpPost("post")]
        public IActionResult Post([FromBody] TechnicalConditionOfRoadViewModel technicalConditionOfRoadData)
        {
            try
            {
                _logger.LogInformation("TechnicalConditionsOfRoadsApiController/Post", "Creating a new technical condition of road...");

                // Проверяем входные данные на null
                if (technicalConditionOfRoadData == null)
                {
                    _logger.LogWarning("TechnicalConditionsOfRoadsApiController/Post", "Incorrect technical condition of road data provided.");
                    return BadRequest("Incorrect technical condition of road data provided");
                }

                // Ищем дорогу по ID дороги во входных данных
                Road? road = _context.Roads.FirstOrDefault(r => r.Id == technicalConditionOfRoadData.RoadId);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning("TechnicalConditionsOfRoadsApiController/Post", $"The road with Id {technicalConditionOfRoadData.RoadId} was not found.");
                    return NotFound($"The road with Id {technicalConditionOfRoadData.RoadId} was not found");
                }

                // Создаем новый объект TechnicalConditionOfRoad на основе входных данных
                TechnicalConditionOfRoad technicalConditionOfRoad = new()
                {
                    Year = technicalConditionOfRoadData.Year,
                    Month = technicalConditionOfRoadData.Month,
                    TechnicalCondition = technicalConditionOfRoadData.TechnicalCondition,
                    RoadId = technicalConditionOfRoadData.RoadId
                };

                // Добавляем новое техническое состояние дороги в контекст данных
                _context.TechnicalConditionsOfRoads.Add(technicalConditionOfRoad);
                _context.SaveChanges();

                _logger.LogInformation("TechnicalConditionsOfRoadsApiController/Post", $"A new technical condition of road with Id {technicalConditionOfRoad.Id} has been successfully created.");

                // Возвращаем успешный результат с Id нового технического состояния дороги
                return Ok(technicalConditionOfRoad.Id);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("TechnicalConditionsOfRoadsApiController/Post", $"Error when creating a new technical condition of road: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Обновляем данные технического состояния дороги
        /// </summary>
        /// <param name="id">Идентификатор технического состояния дороги</param>
        /// <param name="technicalConditionOfRoadData">Новые данные технического состояния дороги</param>
        /// <returns>Данные обновленного технического состояния дороги</returns>
        [HttpPut("put/{id}")]
        public IActionResult Put(int id, [FromBody] TechnicalConditionOfRoadViewModel technicalConditionOfRoadData)
        {
            try
            {
                _logger.LogInformation($"TechnicalConditionsOfRoadsApiController/Put/{id}", $"Updating the technical condition of road with Id {id}...");

                // Ищем техническое состояние дороги по указанному ID в контексте данных
                TechnicalConditionOfRoad? technicalConditionOfRoad = _context.TechnicalConditionsOfRoads.FirstOrDefault(tc => tc.Id == id);

                if (technicalConditionOfRoad == null)
                {
                    // Возвращаем 404 Not Found, если техническое состояние дороги не найдено
                    _logger.LogWarning($"TechnicalConditionsOfRoadsApiController/Put/{id}", $"The technical condition of road with Id {id} was not found.");
                    return NotFound($"The technical condition of road with Id {id} was not found");
                }

                // Ищем дорогу по ID дороги в техническом состоянии дороги
                Road? road = _context.Roads.FirstOrDefault(r => r.Id == technicalConditionOfRoad.RoadId);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning($"TechnicalConditionsOfRoadsApiController/Put/{id}", $"The road with Id {technicalConditionOfRoad.RoadId} was not found.");
                    return NotFound($"The road with Id {technicalConditionOfRoad.RoadId} was not found");
                }

                // Проверяем входные данные на null
                if (technicalConditionOfRoadData == null)
                {
                    _logger.LogWarning($"TechnicalConditionsOfRoadsApiController/Put/{id}", "Incorrect technical condition of road data provided.");
                    return BadRequest("Incorrect technical condition of road data provided");
                }

                // Ищем дорогу по ID дороги во входных данных
                road = _context.Roads.FirstOrDefault(r => r.Id == technicalConditionOfRoadData.RoadId);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning($"TechnicalConditionsOfRoadsApiController/Put/{id}", $"The road with Id {technicalConditionOfRoadData.RoadId} was not found.");
                    return NotFound($"The road with Id {technicalConditionOfRoadData.RoadId} was not found");
                }

                // Обновляем свойства технического состояния дороги на основе входных данных
                technicalConditionOfRoad.Year = technicalConditionOfRoadData.Year;
                technicalConditionOfRoad.Month = technicalConditionOfRoadData.Month;
                technicalConditionOfRoad.TechnicalCondition = technicalConditionOfRoadData.TechnicalCondition;
                technicalConditionOfRoad.RoadId = technicalConditionOfRoadData.RoadId;

                // Сохраняем изменения в контексте данных
                _context.TechnicalConditionsOfRoads.Update(technicalConditionOfRoad);
                _context.SaveChanges();

                _logger.LogInformation($"TechnicalConditionsOfRoadsApiController/Put/{id}", $"The technical condition of road with Id {id} has been successfully updated.");

                // Преобразуем обновленное техническое состояние дороги в JSON объект
                string result = JsonConvert.SerializeObject(technicalConditionOfRoad, Formatting.Indented);

                // Возвращаем успешный результат с JSON объектом обновленного технического состояния дороги
                return Ok(result);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError($"TechnicalConditionsOfRoadsApiController/Put/{id}", $"Error updating the technical condition of road with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удаляем техническое состояние дороги
        /// </summary>
        /// <param name="id">Идентификатор технического состояния дороги</param>
        /// <returns>Количество оставшихся технических состояний дорог</returns>
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation($"TechnicalConditionsOfRoadsApiController/Delete/{id}", $"Deleting a technical condition of road with Id {id}...");

                // Ищем техническое состояние дороги по указанному ID в контексте данных
                TechnicalConditionOfRoad? technicalConditionOfRoad = _context.TechnicalConditionsOfRoads.FirstOrDefault(tc => tc.Id == id);

                if (technicalConditionOfRoad == null)
                {
                    // Возвращаем 404 Not Found, если техническое состояние дороги не найдено
                    _logger.LogWarning($"TechnicalConditionsOfRoadsApiController/Delete/{id}", $"The technical condition of road with Id {id} was not found.");
                    return NotFound($"The technical condition of road with Id {id} was not found");
                }

                // Ищем дорогу по ID дороги в техническом состоянии дороги
                Road? road = _context.Roads.FirstOrDefault(r => r.Id == technicalConditionOfRoad.RoadId);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning($"TechnicalConditionsOfRoadsApiController/Delete/{id}", $"The road with Id {technicalConditionOfRoad.RoadId} was not found.");
                    return NotFound($"The road with Id {technicalConditionOfRoad.RoadId} was not found");
                }

                // Удаляем техническое состояние дороги из контекста данных
                _context.TechnicalConditionsOfRoads.Remove(technicalConditionOfRoad);
                _context.SaveChanges();

                _logger.LogInformation($"TechnicalConditionsOfRoadsApiController/Delete/{id}", $"The technical condition of road with Id {id} has been successfully deleted.");

                // Получаем все оставшиеся технические состояния дорог из контекста данных
                List<TechnicalConditionOfRoad> technicalConditionsOfRoads = _context.TechnicalConditionsOfRoads.ToList();

                // Возвращаем успешный результат с количеством оставшихся технических состояний дорог
                return Ok(technicalConditionsOfRoads.Count);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError($"TechnicalConditionsOfRoadsApiController/Delete/{id}", $"Error when deleting a technical condition of road with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удаляем все технические состояния дорог
        /// </summary>
        /// <returns>Количество оставшихся технических состояний дорог</returns>
        [HttpDelete("delete")]
        public IActionResult Delete()
        {
            try
            {
                _logger.LogInformation("TechnicalConditionsOfRoadsApiController/Delete", "Deleting all the technical conditions of roads...");

                // Получаем все технические состояния дорог из контекста данных
                List<TechnicalConditionOfRoad> technicalConditionsOfRoads = _context.TechnicalConditionsOfRoads.ToList();

                // Удаляем техническое состояние дороги из контекста данных
                _context.TechnicalConditionsOfRoads.RemoveRange(technicalConditionsOfRoads);
                _context.SaveChanges();

                _logger.LogInformation("TechnicalConditionsOfRoadsApiController/Delete", "All technical conditions of roads have been successfully deleted.");

                // Получаем все оставшиеся технические состояния дорог из контекста данных
                technicalConditionsOfRoads = _context.TechnicalConditionsOfRoads.ToList();

                // Возвращаем успешный результат с количеством оставшихся технических состояний дорог
                return Ok(technicalConditionsOfRoads.Count);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("TechnicalConditionsOfRoadsApiController/Delete", $"Error when deleting all technical conditions of roads: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
