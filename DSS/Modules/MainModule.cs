using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using DSS.Models.ViewModels;
using System.Data;

namespace DSS.Modules
{
    public class MainModule
    {
        private readonly DataAnalysisModule _dataAnalysisModule;
        private readonly TechnicalConditionsOfRoadsAnalysisModule _technicalConditionsOfRoadsAnalysisModule;
        private readonly EstimatesAnalysisModule _estimatesAnalysisModule;
        private readonly ApiLogger _logger;

        public MainModule(ApplicationContext context, ILogger<ApiController> logger)
        {
            _dataAnalysisModule = new DataAnalysisModule(context, logger);
            _technicalConditionsOfRoadsAnalysisModule = new TechnicalConditionsOfRoadsAnalysisModule(context, logger);
            _estimatesAnalysisModule = new EstimatesAnalysisModule(context, logger);
            _logger = new ApiLogger(logger);
        }

        public List<(string, DataTable)>? CreatePlans(InputDataViewModel inputData)
        {
            List<(string, DataTable)> plans = new();

            Dictionary<string, double>? initialTechnicalConditionsOfRoads = _technicalConditionsOfRoadsAnalysisModule.GetInitialTechnicalConditionsOfRoads(inputData);

            if (initialTechnicalConditionsOfRoads == null)
            {

            }

            Dictionary<string, double>? predictedTechnicalConditionsOfRoads = _dataAnalysisModule.PredictTechnicalConditionsOfRoads();

            if (predictedTechnicalConditionsOfRoads == null)
            {

            }

            Dictionary<string, double> changesTechnicalConditionsOfRoads = _technicalConditionsOfRoadsAnalysisModule.CalculateChangesTechnicalConditionsOfRoads(initialTechnicalConditionsOfRoads, predictedTechnicalConditionsOfRoads);

            if (changesTechnicalConditionsOfRoads == null)
            {

            }

            Dictionary<string, List<Estimate>>? optimalEstimates = _estimatesAnalysisModule.GetOptimalEstimates(changesTechnicalConditionsOfRoads);

            if (optimalEstimates == null)
            {

            }

            return plans;
        }
    }
}
