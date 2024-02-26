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
                int monthCount = inputData.YearCount * inputData.Months.Count();
                monthCount -= inputData.Months.TakeWhile(month => month != inputData.InitialMonth).Count();

                List<double> budgets = new();

                for (int i = 0; i < monthCount; i++)
                {
                    budgets.Add(inputData.Budget / (monthCount - i));
                    inputData.Budget -= budgets[i];
                }

                return budgets;
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesAnalysisModule/OptimizeOptimalEstimates", $"Error in optimizing optimal estimates: {ex.Message}");
                return null;
            }
        }
    }
}
