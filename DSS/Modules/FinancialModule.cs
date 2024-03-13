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

        public List<double>? CalculateBudget(int currentYear, int currentMonth, InputDataViewModel inputData)
        {
            try
            {
                _logger.LogInformation("FinancialModule/CalculateBudget", "Сalculating the budget...");

                double budget = inputData.Budget;

                int monthCount = inputData.YearCount * inputData.Months.Count();
                monthCount -= inputData.Months.TakeWhile(month => month != inputData.InitialMonth).Count();

                int initialMonth = inputData.Months.ToList().IndexOf(inputData.InitialMonth);

                if (currentYear > inputData.InitialYear || (currentYear == inputData.InitialYear && currentMonth > initialMonth))
                {
                    monthCount -= (currentYear - inputData.InitialYear) * inputData.Months.Count() + (currentMonth - initialMonth);
                }

                List<double> budgets = new();

                for (int i = 0; i < monthCount; i++)
                {
                    budgets.Add(budget / (monthCount - i));
                    budget -= budgets[i];
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

        public double? CalculateCostOfOptimalEstimates(Dictionary<int, List<Estimate>> optimalEstimates)
        {
            try
            {
                _logger.LogInformation("FinancialModule/CalculateCostOfOptimalEstimates", "The cost of optimal estimates has been calculated successfully.");
                return optimalEstimates.Values.Sum(estimates => estimates.Sum(estimate => (double)estimate.Cost));
            }
            catch (Exception ex)
            {
                _logger.LogError("FinancialModule/CalculateCostOfOptimalEstimates", $"Error in calculating the cost of optimal estimates: {ex.Message}");
                return null;
            }
        }
    }
}
