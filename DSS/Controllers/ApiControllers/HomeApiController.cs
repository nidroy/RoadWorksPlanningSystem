using DSS.Models;
using DSS.Models.ViewModels;
using DSS.Modules;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace DSS.Controllers.ApiControllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/home")]
    public class HomeApiController : ApiController
    {
        public HomeApiController(ApplicationContext context, ILogger<ApiController> logger) : base(context, logger)
        {
        }

        [HttpGet("get/plans")]
        public IActionResult GetPlans()
        {
            try
            {
                _logger.LogInformation("HomeApiController/Get/Plans", "Getting plans...");

                // Получаем планы
                List<(string, DataTable)>? plans = MainModule.CreatePlans();

                if (plans == null)
                {
                    // Возвращаем 404 Not Found, если планы не найдены
                    _logger.LogWarning("HomeApiController/Get/Plans", "The plans was not found.");
                    return NotFound("The plans was not found");
                }

                // Преобразуем список планов в JSON массив
                JArray result = new JArray(
                plans.Select(plan => new JObject(new JProperty("Name", plan.Item1),
                    new JProperty("Plan", JArray.FromObject(plan.Item2.AsEnumerable()
                    .Select(row => new JObject(plan.Item2.Columns.Cast<DataColumn>()
                    .Select(col => new JProperty(col.ColumnName, JToken.FromObject(row[col])))
                    ))
                    ))
                ))
                );

                _logger.LogInformation("HomeApiController/Get/Plans", "Plans have been successfully received.");

                // Возвращаем успешный результат с JSON массивом планов
                return Ok(result.ToString());
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("HomeApiController/Get/Plans", $"Error in getting plans: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("get/statistics")]
        public IActionResult GetStatistics(double budget, List<DataTable>? plans)
        {
            try
            {
                _logger.LogInformation("HomeApiController/Get/Statistics", "Getting financial statistics...");

                // Проверяем входные данные на null
                if (plans == null)
                {
                    _logger.LogWarning("HomeApiController/Put/Statistics", "Incorrect plans data provided.");
                    return BadRequest("Incorrect plans data provided");
                }

                // Получаем финансовую статистику
                StatisticsViewModel statistics = StatisticsModule.CalculateFinancialStatistics(budget, plans);

                // Преобразуем финансовую статистику в JSON объект
                string result = JsonConvert.SerializeObject(statistics, Formatting.Indented);

                _logger.LogInformation("HomeApiController/Get/Statistics", "Financial statistics have been successfully received.");

                // Возвращаем успешный результат с JSON объектом финансовой статистики
                return Ok(result);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("HomeApiController/Get/Statistics", $"Error in getting financial statistics: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
