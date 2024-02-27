using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using DSS.Models.ViewModels;
using System.Data;

namespace DSS.Modules
{
    public class MainModule
    {
        private readonly FinancialModule _financialModule;
        private readonly DataAnalysisModule _dataAnalysisModule;
        private readonly TechnicalConditionsOfRoadsAnalysisModule _technicalConditionsOfRoadsAnalysisModule;
        private readonly EstimatesAnalysisModule _estimatesAnalysisModule;
        private readonly ApiLogger _logger;

        public MainModule(ApplicationContext context, ILogger<ApiController> logger)
        {
            _financialModule = new FinancialModule(context, logger);
            _dataAnalysisModule = new DataAnalysisModule(context, logger);
            _technicalConditionsOfRoadsAnalysisModule = new TechnicalConditionsOfRoadsAnalysisModule(context, logger);
            _estimatesAnalysisModule = new EstimatesAnalysisModule(context, logger);
            _logger = new ApiLogger(logger);
        }

        public List<(string, DataTable)>? CreatePlans(InputDataViewModel inputData)
        {
            List<(string, DataTable)> plans = new();

            List<double>? budgets = _financialModule.CalculateBudget(inputData);

            Dictionary<int, double>? initialTechnicalConditionsOfRoads = _technicalConditionsOfRoadsAnalysisModule.GetInitialTechnicalConditionsOfRoads(inputData);

            if (initialTechnicalConditionsOfRoads == null)
            {

            }

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

            // цыкл пока стоимость смет не будет входить в рамки бюджета за месяц

            (int roadId, optimalEstimates) = _estimatesAnalysisModule.OptimizeOptimalEstimates(optimalEstimates);

            if (optimalEstimates == null)
            {

            }

            changesTechnicalConditionsOfRoads.Remove(roadId);

            predictedTechnicalConditionsOfRoads[roadId] -= changesTechnicalConditionsOfRoads[roadId];

            // конец цыкла
            // перенос остатка бюджета на следующий месяц
            // если в последний месяц года остается бюджет то он осваивается самой оптимальной работой 
            // перенос остатка бюджета на первый месяц следующего года
            // формирование планов

            initialTechnicalConditionsOfRoads = predictedTechnicalConditionsOfRoads;

            return plans;
        }
    }
}
