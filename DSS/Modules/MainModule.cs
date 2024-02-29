using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using DSS.Models.ViewModels;

namespace DSS.Modules
{
    public class MainModule
    {
        private readonly FinancialModule _financialModule;
        private readonly TechnicalConditionsOfRoadsAnalysisModule _technicalConditionsOfRoadsAnalysisModule;
        private readonly DataAnalysisModule _dataAnalysisModule;
        private readonly EstimatesAnalysisModule _estimatesAnalysisModule;
        private readonly ApiLogger _logger;

        public MainModule(ApplicationContext context, ILogger<ApiController> logger)
        {
            _financialModule = new FinancialModule(context, logger);
            _technicalConditionsOfRoadsAnalysisModule = new TechnicalConditionsOfRoadsAnalysisModule(context, logger);
            _dataAnalysisModule = new DataAnalysisModule(context, logger);
            _estimatesAnalysisModule = new EstimatesAnalysisModule(context, logger);
            _logger = new ApiLogger(logger);
        }

        public List<(int, string, Dictionary<int, List<Estimate>>)>? CreatePlans(InputDataViewModel inputData)
        {
            List<(int, string, Dictionary<int, List<Estimate>>)> plans = new();

            (int currentYear, int currentMonth, Dictionary<int, double>? initialTechnicalConditionsOfRoads) = _technicalConditionsOfRoadsAnalysisModule.GetInitialTechnicalConditionsOfRoads(inputData);

            if (initialTechnicalConditionsOfRoads == null)
            {

            }

            List<double>? budgets = _financialModule.CalculateBudget(currentYear, currentMonth, inputData);

            if (budgets == null)
            {

            }

            for (int i = 0; i < budgets.Count; i++)
            {
                Dictionary<int, double>? predictedTechnicalConditionsOfRoads = _dataAnalysisModule.PredictTechnicalConditionsOfRoads();

                if (predictedTechnicalConditionsOfRoads == null)
                {

                }

                Dictionary<int, double>? changesTechnicalConditionsOfRoads = _technicalConditionsOfRoadsAnalysisModule.CalculateChangesTechnicalConditionsOfRoads(initialTechnicalConditionsOfRoads, predictedTechnicalConditionsOfRoads);

                if (changesTechnicalConditionsOfRoads == null)
                {

                }

                Dictionary<int, List<Estimate>>? optimalEstimates = _estimatesAnalysisModule.GetOptimalEstimates(changesTechnicalConditionsOfRoads);

                if (optimalEstimates == null)
                {

                }

                double? costOfOptimalEstimates = _financialModule.CalculateCostOfOptimalEstimates(optimalEstimates);

                if (costOfOptimalEstimates == null)
                {

                }

                while (costOfOptimalEstimates > budgets[i])
                {
                    // оптимизировать по приоритету и техническому состоянию
                    (int roadId, optimalEstimates) = _estimatesAnalysisModule.OptimizeOptimalEstimates(optimalEstimates);

                    if (optimalEstimates == null)
                    {

                    }

                    costOfOptimalEstimates = _financialModule.CalculateCostOfOptimalEstimates(optimalEstimates);

                    if (costOfOptimalEstimates == null)
                    {

                    }

                    predictedTechnicalConditionsOfRoads[roadId] -= changesTechnicalConditionsOfRoads[roadId];

                    changesTechnicalConditionsOfRoads[roadId] = 0;
                }

                if (i == budgets.Count - 1)
                {
                    // если в последний месяц года остается бюджет то он осваивается самой оптимальной работой 
                }
                else
                {
                    budgets[i + 1] += budgets[i] - (double)costOfOptimalEstimates;
                }

                plans.Add((currentYear, inputData.Months.ToList()[currentMonth], optimalEstimates));

                // формирование планов

                bool isCreatedTechnicalConditionsOfRoads = _technicalConditionsOfRoadsAnalysisModule.CreateTechnicalConditionsOfRoads(currentYear, inputData.Months.ToList()[currentMonth], predictedTechnicalConditionsOfRoads);

                if (!isCreatedTechnicalConditionsOfRoads)
                {

                }

                initialTechnicalConditionsOfRoads = new(predictedTechnicalConditionsOfRoads);

                currentMonth = (currentMonth + 1) % inputData.Months.Count();
                currentYear += (currentMonth == 0) ? 1 : 0;
            }

            return plans;
        }
    }
}
