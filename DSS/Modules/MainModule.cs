using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using DSS.Models.ViewModels;
using System.Data;

namespace DSS.Modules
{
    public class MainModule
    {
        private readonly TechnicalConditionsOfRoadsAnalysisModule _technicalConditionsOfRoadsAnalysisModule;
        private readonly DataAnalysisModule _dataAnalysisModule;
        private readonly ApiLogger _logger;

        public MainModule(ApplicationContext context, ILogger<ApiController> logger)
        {
            _technicalConditionsOfRoadsAnalysisModule = new TechnicalConditionsOfRoadsAnalysisModule(context, logger);
            _dataAnalysisModule = new DataAnalysisModule(context, logger);
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

            return plans;
        }
    }
}
