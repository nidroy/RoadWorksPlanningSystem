using DSS.Models.ViewModels;
using System.Data;

namespace DSS.Modules
{
    public class StatisticsModule
    {
        public static StatisticsViewModel CalculateFinancialStatistics(double budget, List<DataTable> plans)
        {
            double expenses = 0;

            foreach (DataTable plan in plans)
            {
                DataRow lastRow = plan.Rows[plan.Rows.Count - 1];
                double cost = Convert.ToDouble(lastRow["Стоимость"]);
                expenses += cost;
            }

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
