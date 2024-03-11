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
        private readonly FormPlansModule _formPlansModule;
        private readonly ApiLogger _logger;

        public MainModule(ApplicationContext context, ILogger<ApiController> logger)
        {
            _financialModule = new FinancialModule(context, logger);
            _technicalConditionsOfRoadsAnalysisModule = new TechnicalConditionsOfRoadsAnalysisModule(context, logger);
            _dataAnalysisModule = new DataAnalysisModule(context, logger);
            _estimatesAnalysisModule = new EstimatesAnalysisModule(context, logger);
            _formPlansModule = new FormPlansModule(context, logger);
            _logger = new ApiLogger(logger);
        }

        public List<RoadWorksProgramViewModel>? CreatePlans(InputDataViewModel inputData)
        {
            try
            {
                List<RoadWorksProgramViewModel> plans = new();

                (int currentYear, int currentMonth, Dictionary<int, double>? initialTechnicalConditionsOfRoads) = _technicalConditionsOfRoadsAnalysisModule.GetInitialTechnicalConditionsOfRoads(inputData);

                if (initialTechnicalConditionsOfRoads == null)
                {
                    _logger.LogError("MainModule/CreatePlans", "Error in getting all initial technical conditions of roads");
                    return null;
                }

                List<double>? budgets = _financialModule.CalculateBudget(currentYear, currentMonth, inputData);

                if (budgets == null)
                {
                    _logger.LogError("MainModule/CreatePlans", "Error in calculating the budget");
                    return null;
                }

                for (int i = 0; i < budgets.Count; i++)
                {
                    Dictionary<int, double>? predictedTechnicalConditionsOfRoads = _dataAnalysisModule.PredictTechnicalConditionsOfRoads();

                    if (predictedTechnicalConditionsOfRoads == null)
                    {
                        _logger.LogError("MainModule/CreatePlans", "Error in predicting technical conditions of roads");
                        return null;
                    }

                    Dictionary<int, double>? changesTechnicalConditionsOfRoads = _technicalConditionsOfRoadsAnalysisModule.CalculateChangesTechnicalConditionsOfRoads(initialTechnicalConditionsOfRoads, predictedTechnicalConditionsOfRoads);

                    if (changesTechnicalConditionsOfRoads == null)
                    {
                        _logger.LogError("MainModule/CreatePlans", "Error in calculating the technical conditions of roads");
                        return null;
                    }

                    Dictionary<int, List<Estimate>>? optimalEstimates = _estimatesAnalysisModule.GetOptimalEstimates(changesTechnicalConditionsOfRoads);

                    if (optimalEstimates == null)
                    {
                        _logger.LogError("MainModule/CreatePlans", "Error in getting optimal estimates");
                        return null;
                    }

                    double? costOfOptimalEstimates = _financialModule.CalculateCostOfOptimalEstimates(optimalEstimates);

                    if (costOfOptimalEstimates == null)
                    {
                        _logger.LogError("MainModule/CreatePlans", "Error in calculating the cost of optimal estimates");
                        return null;
                    }

                    while (costOfOptimalEstimates > budgets[i])
                    {
                        (int roadId, double levelOfWorks, optimalEstimates) = _estimatesAnalysisModule.OptimizeOptimalEstimates(optimalEstimates);

                        if (optimalEstimates == null)
                        {
                            _logger.LogError("MainModule/CreatePlans", "Error in optimizing optimal estimates");
                            return null;
                        }

                        costOfOptimalEstimates = _financialModule.CalculateCostOfOptimalEstimates(optimalEstimates);

                        if (costOfOptimalEstimates == null)
                        {
                            _logger.LogError("MainModule/CreatePlans", "Error in calculating the cost of optimal estimates");
                            return null;
                        }

                        predictedTechnicalConditionsOfRoads[roadId] -= levelOfWorks;
                        changesTechnicalConditionsOfRoads[roadId] -= levelOfWorks;
                    }

                    if (i == budgets.Count - 1)
                    {
                        // если в последний месяц года остается бюджет то он осваивается самой оптимальной работой 
                    }
                    else
                    {
                        budgets[i + 1] += budgets[i] - (double)costOfOptimalEstimates;
                    }

                    plans = _formPlansModule.FormPlans(plans, currentYear, inputData.Months.ToList()[currentMonth], optimalEstimates);

                    if (plans == null)
                    {
                        _logger.LogError("MainModule/CreatePlans", "Error in forming the plans");
                        return null;
                    }

                    bool isCreatedTechnicalConditionsOfRoads = _technicalConditionsOfRoadsAnalysisModule.CreateTechnicalConditionsOfRoads(currentYear, inputData.Months.ToList()[currentMonth], predictedTechnicalConditionsOfRoads);

                    if (!isCreatedTechnicalConditionsOfRoads)
                    {
                        _logger.LogError("MainModule/CreatePlans", "Error when creating new technical conditions of roads");
                        return null;
                    }

                    initialTechnicalConditionsOfRoads = new(predictedTechnicalConditionsOfRoads);

                    currentMonth = (currentMonth + 1) % inputData.Months.Count();
                    currentYear += (currentMonth == 0) ? 1 : 0;
                }

                return plans;
            }
            catch (Exception ex)
            {
                _logger.LogError("MainModule/CreatePlans", $"Error when creating plans: {ex.Message}");
                return null;
            }
        }
    }
}
