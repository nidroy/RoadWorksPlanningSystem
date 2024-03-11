using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using DSS.Models.ViewModels;

namespace DSS.Modules
{
    public class FormPlansModule
    {
        private readonly RoadWorksProgramsApiController _roadWorksProgramsApi;
        private readonly ApiLogger _logger;

        public FormPlansModule(ApplicationContext context, ILogger<ApiController> logger)
        {
            _roadWorksProgramsApi = new RoadWorksProgramsApiController(context, logger);
            _logger = new ApiLogger(logger);
        }

        public List<RoadWorksProgramViewModel>? FormPlans(List<RoadWorksProgramViewModel> plans, int year, string month, Dictionary<int, List<Estimate>> optimalEstimates)
        {
            try
            {
                _logger.LogInformation("FormPlansModule/FormPlans", "forming the plans...");

                foreach (var estimates in optimalEstimates)
                {
                    double cost = estimates.Value.Sum(estimate => estimate.Cost ?? 0);
                    List<int> estimatesId = estimates.Value.Select(estimate => estimate.Id).ToList();

                    RoadWorksProgramViewModel plan = new()
                    {
                        Year = year,
                        Month = month,
                        Cost = cost,
                        EstimatesId = estimatesId,
                        RoadId = estimates.Key
                    };

                    _roadWorksProgramsApi.Post(plan);
                    plans.Add(plan);
                }

                _logger.LogInformation("FormPlansModule/FormPlans", "The plans have been formed successfully.");

                return plans;
            }
            catch (Exception ex)
            {
                _logger.LogError("FormPlansModule/FormPlans", $"Error in forming the plans: {ex.Message}");
                return null;
            }
        }
    }
}
