using DSS.Models;
using DSS.Models.ViewModels;

namespace DSS.Modules
{
    public class StatisticsModule
    {
        public static StatisticsViewModel CalculateFinancialStatistics(double budget, List<(int, string, Dictionary<int, List<Estimate>>)> plans)
        {
            double expenses = plans.Sum(plan => plan.Item3.Values.Sum(estimates => estimates.Sum(estimate => (double)estimate.Cost)));
            double balance = budget - expenses;

            StatisticsViewModel statistics = new()
            {
                Budget = budget,
                Expenses = expenses,
                Balance = balance
            };

            return statistics;
        }
    }
}
