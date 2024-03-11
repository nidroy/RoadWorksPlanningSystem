using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using DSS.Models.ViewModels;

namespace DSS.Modules
{
    public class StatisticsModule
    {
        private readonly ApiLogger _logger;

        public StatisticsModule(ApplicationContext context, ILogger<ApiController> logger)
        {
            _logger = new ApiLogger(logger);
        }

        public StatisticsViewModel? CalculateFinancialStatistics(double budget, List<RoadWorksProgramViewModel> roadWorksPrograms)
        {
            try
            {
                _logger.LogInformation("StatisticsModule/CalculateFinancialStatistics", "Calculating financial statistics...");

                double expenses = roadWorksPrograms.Sum(r => r.Cost ?? 0);
                double balance = budget - expenses;

                StatisticsViewModel statistics = new()
                {
                    Budget = budget,
                    Expenses = expenses,
                    Balance = balance
                };

                _logger.LogInformation("StatisticsModule/CalculateFinancialStatistics", "The financial statistics have been successfully calculated.");

                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError("StatisticsModule/CalculateFinancialStatistics", $"Error in calculating the financial statistics: {ex.Message}");
                return null;
            }
        }
    }
}
