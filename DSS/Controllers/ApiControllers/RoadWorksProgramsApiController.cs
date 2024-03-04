using DSS.Models;
using DSS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DSS.Controllers.ApiControllers
{
    [ApiController]
    [Route("api/roadWorksPrograms")]
    public class RoadWorksProgramsApiController : ApiController
    {
        public RoadWorksProgramsApiController(ApplicationContext context, ILogger<ApiController> logger) : base(context, logger)
        {
        }

        /// <summary>
        /// Получаем данные всех программ дорожных работ
        /// </summary>
        /// <returns>Данные всех программ дорожных работ</returns>
        [HttpGet("get")]
        public IActionResult Get()
        {
            try
            {
                _logger.LogInformation("RoadWorksProgramsApiController/Get", "Getting all the road works programs...");

                // Получаем все программы дорожных работ из контекста данных
                List<RoadWorksProgram> roadWorksPrograms = _context.RoadWorksPrograms.ToList();

                // Получаем все сметы из контекста данных
                List<Estimate> estimates = _context.Estimates.ToList();

                // Преобразуем список программ дорожных работ в JSON массив
                JArray result = new JArray();

                foreach (var roadWorksProgram in roadWorksPrograms)
                {
                    // Ищем дорогу по ID дороги в программе дорожных работ
                    Road? road = _context.Roads.FirstOrDefault(r => r.Id == roadWorksProgram.RoadId);

                    if (road == null)
                    {
                        // Возвращаем 404 Not Found, если дорога не найдена
                        _logger.LogWarning("RoadWorksProgramsApiController/Get", $"The road with Id {roadWorksProgram.RoadId} was not found.");
                        return NotFound($"The road with Id {roadWorksProgram.RoadId} was not found");
                    }

                    // Ищем сметы по ID смет в программе дорожных работ
                    List<RoadWorksProgramToEstimate>? roadWorksProgramsToEstimates = _context.RoadWorksProgramsToEstimates
                        .Where(pe => pe.RoadWorksProgramId == roadWorksProgram.Id)
                        .ToList();

                    if (roadWorksProgramsToEstimates == null)
                    {
                        // Возвращаем 404 Not Found, если ID смет не найдены
                        _logger.LogWarning("RoadWorksProgramsApiController/Get", "The estimates Id were not found.");
                        return NotFound("The estimates Id were not found");
                    }

                    estimates = estimates
                        .Where(e => roadWorksProgramsToEstimates.Any(pe => pe.EstimateId == e.Id))
                        .ToList();

                    if (estimates == null)
                    {
                        // Возвращаем 404 Not Found, если сметы не найдены
                        _logger.LogWarning("RoadWorksProgramsApiController/Get", "The estimates were not found.");
                        return NotFound("The estimates were not found");
                    }

                    result.Add(new JObject
                    {
                        ["Id"] = roadWorksProgram.Id,
                        ["Year"] = roadWorksProgram.Year,
                        ["Month"] = roadWorksProgram.Month,
                        ["Cost"] = roadWorksProgram.Cost,
                        ["Estimates"] = new JArray(estimates.Select(e => new JObject
                        {
                            ["Id"] = e.Id,
                            ["Name"] = e.Name,
                            ["LevelOfWorks"] = e.LevelOfWorks,
                            ["Cost"] = e.Cost,
                            ["Link"] = e.Link
                        })),
                        ["RoadId"] = roadWorksProgram.RoadId,
                        ["Road"] = new JObject
                        {
                            ["Id"] = road.Id,
                            ["Number"] = road.Number,
                            ["Priority"] = road.Priority,
                            ["LinkToPassport"] = road.LinkToPassport
                        }
                    });
                }

                _logger.LogInformation("RoadWorksProgramsApiController/Get", "All road works programs have been successfully received.");

                // Возвращаем успешный результат с JSON массивом программ дорожных работ
                return Ok(result.ToString());
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("RoadWorksProgramsApiController/Get", $"Error in getting all road works programs: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получаем данные программы дорожных работ по id
        /// </summary>
        /// <param name="id">Идентификатор программы дорожных работ</param>
        /// <returns>Данные программы дорожных работ</returns>
        [HttpGet("get/{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                _logger.LogInformation($"RoadWorksProgramsApiController/Get/{id}", $"Getting a road works program with Id {id}...");

                // Ищем программу дорожных работ по указанному ID в контексте данных
                RoadWorksProgram? roadWorksProgram = _context.RoadWorksPrograms.FirstOrDefault(p => p.Id == id);

                if (roadWorksProgram == null)
                {
                    // Возвращаем 404 Not Found, если программа дорожных работ не найдена
                    _logger.LogWarning($"RoadWorksProgramsApiController/Get/{id}", $"The road works program with Id {id} was not found.");
                    return NotFound($"The road works program with Id {id} was not found");
                }

                // Ищем дорогу по ID дороги в программе дорожных работ
                Road? road = _context.Roads.FirstOrDefault(r => r.Id == roadWorksProgram.RoadId);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning($"RoadWorksProgramsApiController/Get/{id}", $"The road with Id {roadWorksProgram.RoadId} was not found.");
                    return NotFound($"The road with Id {roadWorksProgram.RoadId} was not found");
                }

                // Ищем сметы по ID смет в программе дорожных работ
                List<RoadWorksProgramToEstimate>? roadWorksProgramsToEstimates = _context.RoadWorksProgramsToEstimates
                        .Where(pe => pe.RoadWorksProgramId == roadWorksProgram.Id)
                        .ToList();

                if (roadWorksProgramsToEstimates == null)
                {
                    // Возвращаем 404 Not Found, если ID смет не найдены
                    _logger.LogWarning($"RoadWorksProgramsApiController/Get/{id}", "The estimates Id were not found.");
                    return NotFound("The estimates Id were not found");
                }

                List<Estimate> estimates = _context.Estimates
                    .Where(e => roadWorksProgramsToEstimates.Any(pe => pe.EstimateId == e.Id))
                    .ToList();

                if (estimates == null)
                {
                    // Возвращаем 404 Not Found, если сметы не найдены
                    _logger.LogWarning($"RoadWorksProgramsApiController/Get/{id}", "The estimates were not found.");
                    return NotFound("The estimates were not found");
                }

                // Преобразуем найденную программу дорожных работ в JSON объект
                JObject result = new()
                {
                    ["Id"] = roadWorksProgram.Id,
                    ["Year"] = roadWorksProgram.Year,
                    ["Month"] = roadWorksProgram.Month,
                    ["Cost"] = roadWorksProgram.Cost,
                    ["Estimates"] = new JArray(estimates.Select(e => new JObject
                    {
                        ["Id"] = e.Id,
                        ["Name"] = e.Name,
                        ["LevelOfWorks"] = e.LevelOfWorks,
                        ["Cost"] = e.Cost,
                        ["Link"] = e.Link
                    })),
                    ["RoadId"] = roadWorksProgram.RoadId,
                    ["Road"] = new JObject
                    {
                        ["Id"] = road.Id,
                        ["Number"] = road.Number,
                        ["Priority"] = road.Priority,
                        ["LinkToPassport"] = road.LinkToPassport
                    }
                };

                _logger.LogInformation($"RoadWorksProgramsApiController/Get/{id}", $"The road works program with Id {id} was successfully received.");

                // Возвращаем успешный результат с JSON объектом программы дорожных работ
                return Ok(result.ToString());
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError($"RoadWorksProgramsApiController/Get/{id}", $"Error when getting a road works program with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Создаем новую программу дорожных работ
        /// </summary>
        /// <param name="roadWorksProgramData">Данные новой программы дорожных работ</param>
        /// <returns>Идентификатор новой программы дорожных работ</returns>
        [HttpPost("post")]
        public IActionResult Post([FromBody] RoadWorksProgramViewModel roadWorksProgramData)
        {
            try
            {
                _logger.LogInformation("RoadWorksProgramsApiController/Post", "Creating a new road works program...");

                // Проверяем входные данные на null
                if (roadWorksProgramData == null)
                {
                    _logger.LogWarning("RoadWorksProgramsApiController/Post", "Incorrect road works program data provided.");
                    return BadRequest("Incorrect road works program data provided");
                }

                // Ищем дорогу по ID дороги во входных данных
                Road? road = _context.Roads.FirstOrDefault(r => r.Id == roadWorksProgramData.RoadId);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning("RoadWorksProgramsApiController/Post", $"The road with Id {roadWorksProgramData.RoadId} was not found.");
                    return NotFound($"The road with Id {roadWorksProgramData.RoadId} was not found");
                }

                // Создаем новый объект RoadWorksProgram на основе входных данных
                RoadWorksProgram roadWorksProgram = new()
                {
                    Year = roadWorksProgramData.Year,
                    Month = roadWorksProgramData.Month,
                    Cost = roadWorksProgramData.Cost,
                    RoadId = roadWorksProgramData.RoadId
                };

                // Добавляем новую программу дорожных работ в контекст данных
                _context.RoadWorksPrograms.Add(roadWorksProgram);
                _context.SaveChanges();

                // Добавляем ID смет для новой программы дорожных работ
                foreach (var estimateId in roadWorksProgramData.EstimatesId)
                {
                    RoadWorksProgramToEstimate roadWorksProgramToEstimate = new()
                    {
                        RoadWorksProgramId = roadWorksProgram.Id,
                        EstimateId = estimateId
                    };

                    _context.RoadWorksProgramsToEstimates.Add(roadWorksProgramToEstimate);
                    _context.SaveChanges();
                }

                _logger.LogInformation("RoadWorksProgramsApiController/Post", $"A new road works program with Id {roadWorksProgram.Id} has been successfully created.");

                // Возвращаем успешный результат с Id новой программы дорожных работ
                return Ok(roadWorksProgram.Id);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("RoadWorksProgramsApiController/Post", $"Error when creating a new road works program: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Обновляем данные программы дорожных работ
        /// </summary>
        /// <param name="id">Идентификатор программы дорожных работ</param>
        /// <param name="roadWorksProgramData">Новые данные программы дорожных работ</param>
        /// <returns>Данные обновленной программы дорожных работ</returns>
        [HttpPut("put/{id}")]
        public IActionResult Put(int id, [FromBody] RoadWorksProgramViewModel roadWorksProgramData)
        {
            try
            {
                _logger.LogInformation($"RoadWorksProgramsApiController/Put/{id}", $"Updating the road works program with Id {id}...");

                // Ищем программу дорожных работ по указанному ID в контексте данных
                RoadWorksProgram? roadWorksProgram = _context.RoadWorksPrograms.FirstOrDefault(p => p.Id == id);

                if (roadWorksProgram == null)
                {
                    // Возвращаем 404 Not Found, если программа дорожных работ не найдена
                    _logger.LogWarning($"RoadWorksProgramsApiController/Put/{id}", $"The road works program with Id {id} was not found.");
                    return NotFound($"The road works program with Id {id} was not found");
                }

                // Ищем дорогу по ID дороги в программе дорожных работ
                Road? road = _context.Roads.FirstOrDefault(r => r.Id == roadWorksProgram.RoadId);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning($"RoadWorksProgramsApiController/Put/{id}", $"The road with Id {roadWorksProgram.RoadId} was not found.");
                    return NotFound($"The road with Id {roadWorksProgram.RoadId} was not found");
                }

                // Проверяем входные данные на null
                if (roadWorksProgramData == null)
                {
                    _logger.LogWarning($"RoadWorksProgramsApiController/Put/{id}", "Incorrect road works program data provided.");
                    return BadRequest("Incorrect road works program data provided");
                }

                // Ищем дорогу по ID дороги во входных данных
                road = _context.Roads.FirstOrDefault(r => r.Id == roadWorksProgramData.RoadId);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning($"RoadWorksProgramsApiController/Put/{id}", $"The road with Id {roadWorksProgramData.RoadId} was not found.");
                    return NotFound($"The road with Id {roadWorksProgramData.RoadId} was not found");
                }

                // Обновляем свойства программы дорожных работ на основе входных данных
                roadWorksProgram.Year = roadWorksProgramData.Year;
                roadWorksProgram.Month = roadWorksProgramData.Month;
                roadWorksProgram.Cost = roadWorksProgramData.Cost;
                roadWorksProgram.RoadId = roadWorksProgramData.RoadId;

                // Сохраняем изменения в контексте данных
                _context.RoadWorksPrograms.Update(roadWorksProgram);
                _context.SaveChanges();

                // Ищем сметы по ID смет в программе дорожных работ
                List<RoadWorksProgramToEstimate>? roadWorksProgramsToEstimates = _context.RoadWorksProgramsToEstimates
                        .Where(pe => pe.RoadWorksProgramId == roadWorksProgram.Id)
                        .ToList();

                if (roadWorksProgramsToEstimates == null)
                {
                    // Возвращаем 404 Not Found, если ID смет не найдены
                    _logger.LogWarning($"RoadWorksProgramsApiController/Put/{id}", "The estimates Id were not found.");
                    return NotFound("The estimates Id were not found");
                }

                // Удаляем ID смет для программы дорожных работ из контекста данных
                foreach (var roadWorksProgramToEstimate in roadWorksProgramsToEstimates)
                {
                    _context.RoadWorksProgramsToEstimates.Remove(roadWorksProgramToEstimate);
                    _context.SaveChanges();
                }

                // Обновляем ID смет для программы дорожных работ
                foreach (var estimateId in roadWorksProgramData.EstimatesId)
                {
                    RoadWorksProgramToEstimate roadWorksProgramToEstimate = new()
                    {
                        RoadWorksProgramId = roadWorksProgram.Id,
                        EstimateId = estimateId
                    };

                    _context.RoadWorksProgramsToEstimates.Add(roadWorksProgramToEstimate);
                    _context.SaveChanges();
                }

                _logger.LogInformation($"RoadWorksProgramsApiController/Put/{id}", $"The road works program with Id {id} has been successfully updated.");

                // Ищем сметы по ID смет в программе дорожных работ
                roadWorksProgramsToEstimates = _context.RoadWorksProgramsToEstimates
                        .Where(pe => pe.RoadWorksProgramId == roadWorksProgram.Id)
                        .ToList();

                if (roadWorksProgramsToEstimates == null)
                {
                    // Возвращаем 404 Not Found, если ID смет не найдены
                    _logger.LogWarning($"RoadWorksProgramsApiController/Put/{id}", "The estimates Id were not found.");
                    return NotFound("The estimates Id were not found");
                }

                List<Estimate> estimates = _context.Estimates
                    .Where(e => roadWorksProgramsToEstimates.Any(pe => pe.EstimateId == e.Id))
                    .ToList();

                if (estimates == null)
                {
                    // Возвращаем 404 Not Found, если сметы не найдены
                    _logger.LogWarning($"RoadWorksProgramsApiController/Put/{id}", "The estimates were not found.");
                    return NotFound("The estimates were not found");
                }

                // Преобразуем обновленную программу дорожных работ в JSON объект
                JObject result = new()
                {
                    ["Id"] = roadWorksProgram.Id,
                    ["Year"] = roadWorksProgram.Year,
                    ["Month"] = roadWorksProgram.Month,
                    ["Cost"] = roadWorksProgram.Cost,
                    ["Estimates"] = new JArray(estimates.Select(e => new JObject
                    {
                        ["Id"] = e.Id,
                        ["Name"] = e.Name,
                        ["LevelOfWorks"] = e.LevelOfWorks,
                        ["Cost"] = e.Cost,
                        ["Link"] = e.Link
                    })),
                    ["RoadId"] = roadWorksProgram.RoadId,
                    ["Road"] = new JObject
                    {
                        ["Id"] = road.Id,
                        ["Number"] = road.Number,
                        ["Priority"] = road.Priority,
                        ["LinkToPassport"] = road.LinkToPassport
                    }
                };

                // Возвращаем успешный результат с JSON объектом обновленной программы дорожных работ
                return Ok(result.ToString());
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError($"RoadWorksProgramsApiController/Put/{id}", $"Error updating the road works program with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Удаляем программу дорожных работ
        /// </summary>
        /// <param name="id">Идентификатор программы дорожных работ</param>
        /// <returns>Количество оставшихся программ дорожных работ</returns>
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation($"RoadWorksProgramsApiController/Delete/{id}", $"Deleting a road works program with Id {id}...");

                // Ищем программу дорожных работ по указанному ID в контексте данных
                RoadWorksProgram? roadWorksProgram = _context.RoadWorksPrograms.FirstOrDefault(p => p.Id == id);

                if (roadWorksProgram == null)
                {
                    // Возвращаем 404 Not Found, если дорожная работа не найдена
                    _logger.LogWarning($"RoadWorksProgramsApiController/Delete/{id}", $"The road works program with Id {id} was not found.");
                    return NotFound($"The road works program with Id {id} was not found");
                }

                // Ищем дорогу по ID дороги в программе дорожных работ
                Road? road = _context.Roads.FirstOrDefault(r => r.Id == roadWorksProgram.RoadId);

                if (road == null)
                {
                    // Возвращаем 404 Not Found, если дорога не найдена
                    _logger.LogWarning($"RoadWorksProgramsApiController/Delete/{id}", $"The road with Id {roadWorksProgram.RoadId} was not found.");
                    return NotFound($"The road with Id {roadWorksProgram.RoadId} was not found");
                }

                // Ищем сметы по ID смет в программе дорожных работ
                List<RoadWorksProgramToEstimate>? roadWorksProgramsToEstimates = _context.RoadWorksProgramsToEstimates
                        .Where(pe => pe.RoadWorksProgramId == roadWorksProgram.Id)
                        .ToList();

                if (roadWorksProgramsToEstimates == null)
                {
                    // Возвращаем 404 Not Found, если ID смет не найдены
                    _logger.LogWarning($"RoadWorksProgramsApiController/Delete/{id}", "The estimates Id were not found.");
                    return NotFound("The estimates Id were not found");
                }

                // Удаляем ID смет для программы дорожных работ из контекста данных
                foreach (var roadWorksProgramToEstimate in roadWorksProgramsToEstimates)
                {
                    _context.RoadWorksProgramsToEstimates.Remove(roadWorksProgramToEstimate);
                    _context.SaveChanges();
                }

                // Удаляем программу дорожных работ из контекста данных
                _context.RoadWorksPrograms.Remove(roadWorksProgram);
                _context.SaveChanges();

                _logger.LogInformation($"RoadWorksProgramsApiController/Delete/{id}", $"The road works program with Id {id} has been successfully deleted.");

                // Получаем все оставшиеся программы дорожных работ из контекста данных
                List<RoadWorksProgram> roadWorksPrograms = _context.RoadWorksPrograms.ToList();

                // Возвращаем успешный результат с количеством оставшихся технических состояний дорог
                return Ok(roadWorksPrograms.Count);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError($"RoadWorksProgramsApiController/Delete/{id}", $"Error when deleting a road works program with Id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
