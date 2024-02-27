using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DSS.Modules
{
    public class EstimatesAnalysisModule
    {
        private readonly RoadsApiController _roadsApi;
        private readonly EstimatesApiController _estimatesApi;
        private readonly ApiLogger _logger;

        public EstimatesAnalysisModule(ApplicationContext context, ILogger<ApiController> logger)
        {
            _roadsApi = new RoadsApiController(context, logger);
            _estimatesApi = new EstimatesApiController(context, logger);
            _logger = new ApiLogger(logger);
        }

        public (int, Dictionary<int, List<Estimate>>?) OptimizeOptimalEstimates(Dictionary<int, List<Estimate>> optimalEstimates)
        {
            try
            {
                _logger.LogInformation("EstimatesAnalysisModule/OptimizeOptimalEstimates", "Optimization of optimal estimates...");

                var roadId = optimalEstimates.Where(optimalEstimates => optimalEstimates.Value.Any())
                    .OrderByDescending(optimalEstimates => optimalEstimates.Value.First().Road.Priority)
                    .ThenBy(optimalEstimates => optimalEstimates.Value.Sum(estimate => estimate.Cost))
                    .Select(optimalEstimates => optimalEstimates.Key)
                    .FirstOrDefault();

                optimalEstimates.Remove(roadId);

                _logger.LogInformation("EstimatesAnalysisModule/OptimizeOptimalEstimates", "Optimal estimates have been successfully optimized.");

                return (roadId, optimalEstimates);
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesAnalysisModule/OptimizeOptimalEstimates", $"Error in optimizing optimal estimates: {ex.Message}");
                return (0, null);
            }
        }

        public Dictionary<int, List<Estimate>>? GetOptimalEstimates(Dictionary<int, double> changesTechnicalConditionsOfRoads)
        {
            try
            {
                _logger.LogInformation("EstimatesAnalysisModule/GetOptimalEstimates", "Reading all estimates...");

                var result = _estimatesApi.Get();
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("EstimatesAnalysisModule/GetOptimalEstimates", "Error on the API side of the controller.");
                    return null;
                }

                var estimates = JsonConvert.DeserializeObject<IEnumerable<Estimate>>(value.ToString());

                _logger.LogInformation("EstimatesAnalysisModule/GetOptimalEstimates", "All estimates have been successfully read.");

                _logger.LogInformation("EstimatesAnalysisModule/GetOptimalEstimates", "Reading all roads...");

                result = _roadsApi.Get();
                statusCode = ((ObjectResult)result).StatusCode;
                value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("EstimatesAnalysisModule/GetOptimalEstimates", "Error on the API side of the controller.");
                    return null;
                }

                var roads = JsonConvert.DeserializeObject<IEnumerable<Road>>(value.ToString());

                _logger.LogInformation("EstimatesAnalysisModule/GetOptimalEstimates", "All roads have been successfully read.");

                _logger.LogInformation("EstimatesAnalysisModule/GetOptimalEstimates", "Getting optimal estimates...");

                estimates = estimates.GroupBy(e => e.LevelOfWorks)
                    .Select(estimates => estimates.OrderBy(e => e.Cost).First());

                var levelsOfWorks = estimates.Select(e => e.LevelOfWorks)
                    .Where(LevelOfWorks => LevelOfWorks.HasValue)
                    .Select(LevelOfWorks => LevelOfWorks.Value)
                    .Distinct()
                    .ToList();

                var optimalEstimates = roads.ToDictionary(
                    road => road.Id,
                    road => GetOptimalEstimatesForOneRoad(changesTechnicalConditionsOfRoads[road.Id], levelsOfWorks, estimates));

                _logger.LogInformation("EstimatesAnalysisModule/GetOptimalEstimates", "Optimal estimates have been successfully received.");

                return optimalEstimates;
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesAnalysisModule/GetOptimalEstimates", $"Error in getting optimal estimates: {ex.Message}");
                return null;
            }
        }

        private List<Estimate>? GetOptimalEstimatesForOneRoad(double changeTechnicalConditionOfRoad, List<double> levelsOfWorks, IEnumerable<Estimate> estimates)
        {
            try
            {
                _logger.LogInformation("EstimatesAnalysisModule/GetOptimalEstimatesForOneRoad", "Getting optimal estimates for one road...");

                List<List<double>>? combinationsOfLevelsOfWorks = GetCombinationsOfLevelsOfWorks(changeTechnicalConditionOfRoad, levelsOfWorks);

                if (combinationsOfLevelsOfWorks == null)
                {
                    _logger.LogWarning("EstimatesAnalysisModule/GetOptimalEstimates", "Error in getting combinations of levels of works.");
                    return null;
                }

                var optimalEstimatesForOneRoad = combinationsOfLevelsOfWorks
                    .Select(combinationOfLevelsOfWorks =>
                    {
                        double costOfCombinationOfLevelsOfWorks = (double)combinationOfLevelsOfWorks
                        .Sum(levelOfWorks => estimates.Where(estimate => estimate.LevelOfWorks == levelOfWorks)
                        .Sum(estimate => estimate.Cost));

                        return (combinationOfLevelsOfWorks, costOfCombinationOfLevelsOfWorks);
                    })
                    .OrderBy(combinationOfLevelsOfWorks => combinationOfLevelsOfWorks.costOfCombinationOfLevelsOfWorks)
                    .FirstOrDefault().combinationOfLevelsOfWorks
                    .SelectMany(levelOfWorks => estimates.Where(estimate => estimate.LevelOfWorks == levelOfWorks))
                    .ToList();

                _logger.LogInformation("EstimatesAnalysisModule/GetOptimalEstimatesForOneRoad", "Optimal estimates for one road have been successfully received.");

                return optimalEstimatesForOneRoad;
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesAnalysisModule/GetOptimalEstimatesForOneRoad", $"Error in getting optimal estimates for one road: {ex.Message}");
                return null;
            }
        }

        private List<List<double>>? GetCombinationsOfLevelsOfWorks(double changeTechnicalConditionOfRoad, List<double> levelsOfWorks)
        {
            try
            {
                _logger.LogInformation("EstimatesAnalysisModule/GetCombinationsOfLevelsOfWorks", "Getting all the combinations of levels of works...");

                List<List<double>> combinationsOfLevelsOfWorks = new();
                List<double> combinationOfLevelsOfWorks = new();

                GetCombinationOfLevelsOfWorks(changeTechnicalConditionOfRoad, levelsOfWorks, 0, combinationOfLevelsOfWorks, combinationsOfLevelsOfWorks);

                _logger.LogInformation("EstimatesAnalysisModule/GetCombinationsOfLevelsOfWorks", "All combinations of levels of works have been successfully received.");

                return combinationsOfLevelsOfWorks;
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesAnalysisModule/GetCombinationsOfLevelsOfWorks", $"Error in getting all combinations of levels of works: {ex.Message}");
                return null;
            }
        }

        private void GetCombinationOfLevelsOfWorks(double? changeTechnicalConditionOfRoad, List<double> levelsOfWorks, int initialIndexOfLevelOfWorks, List<double> combinationOfLevelsOfWorks, List<List<double>> combinationsOfLevelsOfWorks)
        {
            try
            {
                if (changeTechnicalConditionOfRoad == 0)
                {
                    combinationsOfLevelsOfWorks.Add(new List<double>(combinationOfLevelsOfWorks));

                    _logger.LogInformation("EstimatesAnalysisModule/GetCombinationOfLevelsOfWorks", "The combination of levels of works was successfully received.");

                    return;
                }

                for (int indexOfLevelOfWorks = initialIndexOfLevelOfWorks; indexOfLevelOfWorks < levelsOfWorks.Count; indexOfLevelOfWorks++)
                {
                    double levelOfWorks = levelsOfWorks[indexOfLevelOfWorks];

                    if (changeTechnicalConditionOfRoad >= levelOfWorks)
                    {
                        combinationOfLevelsOfWorks.Add(levelOfWorks);
                        GetCombinationOfLevelsOfWorks(changeTechnicalConditionOfRoad - levelOfWorks, levelsOfWorks, indexOfLevelOfWorks, combinationOfLevelsOfWorks, combinationsOfLevelsOfWorks);
                        combinationOfLevelsOfWorks.RemoveAt(combinationOfLevelsOfWorks.Count - 1);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("EstimatesAnalysisModule/GetCombinationOfLevelsOfWorks", $"Error when getting a combination of levels of works: {ex.Message}");
                return;
            }
        }
    }
}
