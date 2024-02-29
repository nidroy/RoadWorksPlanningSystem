using DSS.Models;
using DSS.Models.ViewModels;
using DSS.Modules;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DSS.Controllers.ApiControllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/home")]
    public class HomeApiController : ApiController
    {
        private readonly MainModule _mainModule;

        public HomeApiController(ApplicationContext context, ILogger<ApiController> logger) : base(context, logger)
        {
            _mainModule = new(context, logger);
        }

        /// <summary>
        /// Получаем данные планов
        /// </summary>
        /// <param name="inputData">Входные данные</param>
        /// <returns>Данные планов</returns>
        [HttpGet("get/plans")]
        public IActionResult GetPlans(InputDataViewModel inputData)
        {
            try
            {
                _logger.LogInformation("HomeApiController/Get/Plans", "Getting plans...");

                // Получаем планы
                List<(int, string, Dictionary<int, List<Estimate>>)>? plans = _mainModule.CreatePlans(inputData);

                if (plans == null)
                {
                    // Возвращаем 404 Not Found, если планы не найдены
                    _logger.LogWarning("HomeApiController/Get/Plans", "The plans was not found.");
                    return NotFound("The plans was not found");
                }

                // Преобразуем список планов в JSON объект
                string result = JsonConvert.SerializeObject(plans, Formatting.Indented);

                _logger.LogInformation("HomeApiController/Get/Plans", "Plans have been successfully received.");

                // Возвращаем успешный результат с JSON объектом планов
                return Ok(result);
            }
            catch (Exception ex)
            {
                // В случае ошибки логируем и возвращаем 500 Internal Server Error
                _logger.LogError("HomeApiController/Get/Plans", $"Error in getting plans: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Получаем финансовую статистику
        /// </summary>
        /// <param name="budget">Бюджет</param>
        /// <param name="plans">Данные планов</param>
        /// <returns>Финансовая статистика</returns>
        [HttpGet("get/statistics")]
        public IActionResult GetStatistics(double budget, List<(int, string, Dictionary<int, List<Estimate>>)> plans)
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
