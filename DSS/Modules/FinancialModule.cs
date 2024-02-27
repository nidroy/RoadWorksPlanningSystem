using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using DSS.Models.ViewModels;

namespace DSS.Modules
{
    public class FinancialModule
    {
        private readonly ApiLogger _logger;

        public FinancialModule(ApplicationContext context, ILogger<ApiController> logger)
        {
            _logger = new ApiLogger(logger);
        }

        public List<double>? CalculateBudget(InputDataViewModel inputData)
        {
            try
            {
                _logger.LogInformation("FinancialModule/CalculateBudget", "Сalculating the budget...");

                int monthCount = inputData.YearCount * inputData.Months.Count();
                monthCount -= inputData.Months.TakeWhile(month => month != inputData.InitialMonth).Count();

                List<double> budgets = new();

                for (int i = 0; i < monthCount; i++)
                {
                    budgets.Add(inputData.Budget / (monthCount - i));
                    inputData.Budget -= budgets[i];
                }

                _logger.LogInformation("FinancialModule/CalculateBudget", "The budget has been calculated successfully.");

                return budgets;
            }
            catch (Exception ex)
            {
                _logger.LogError("FinancialModule/CalculateBudget", $"Error in calculating the budget: {ex.Message}");
                return null;
            }
        }
    }
}
