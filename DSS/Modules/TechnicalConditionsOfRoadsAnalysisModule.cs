using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using DSS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DSS.Modules
{
    public class TechnicalConditionsOfRoadsAnalysisModule
    {
        private readonly TechnicalConditionsOfRoadsApiController _technicalConditionsOfRoadsApi;
        private readonly ApiLogger _logger;

        public TechnicalConditionsOfRoadsAnalysisModule(ApplicationContext context, ILogger<ApiController> logger)
        {
            _technicalConditionsOfRoadsApi = new TechnicalConditionsOfRoadsApiController(context, logger);
            _logger = new ApiLogger(logger);
        }

        public Dictionary<int, double>? GetInitialTechnicalConditionsOfRoads(InputDataViewModel inputData)
        {
            try
            {
                _logger.LogInformation("TechnicalConditionsOfRoadsAnalysisModule/GetInitialTechnicalConditionsOfRoads", "Reading all technical conditions of roads...");

                var result = _technicalConditionsOfRoadsApi.Get();
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("TechnicalConditionsOfRoadsAnalysisModule/GetInitialTechnicalConditionsOfRoads", "Error on the API side of the controller.");
                    return null;
                }

                var technicalConditionsOfRoads = JsonConvert.DeserializeObject<IEnumerable<TechnicalConditionOfRoad>>(value.ToString());

                _logger.LogInformation("TechnicalConditionsOfRoadsAnalysisModule/GetInitialTechnicalConditionsOfRoads", "All technical conditions of roads have been successfully read.");

                _logger.LogInformation("TechnicalConditionsOfRoadsAnalysisModule/GetInitialTechnicalConditionsOfRoads", "Getting all initial technical conditions of roads...");

                Dictionary<int, double>? initialTechnicalConditionsOfRoads = new();

                technicalConditionsOfRoads = technicalConditionsOfRoads.GroupBy(tc => tc.RoadId)
                    .Select(technicalConditionsOfRoads => technicalConditionsOfRoads.OrderByDescending(tc => tc.Year)
                    .ThenByDescending(tc => Array.IndexOf(inputData.Months.ToArray(), tc.Month))
                    .First());

                if (technicalConditionsOfRoads.Count() == 0)
                {
                    _logger.LogWarning("TechnicalConditionsOfRoadsAnalysisModule/GetInitialTechnicalConditionsOfRoads", "The technical conditions of roads were not found.");
                    return null;
                }

                foreach (var technicalConditionOfRoad in technicalConditionsOfRoads)
                {
                    initialTechnicalConditionsOfRoads.Add(technicalConditionOfRoad.RoadId, (double)technicalConditionOfRoad.TechnicalCondition);
                }

                _logger.LogInformation("TechnicalConditionsOfRoadsAnalysisModule/GetInitialTechnicalConditionsOfRoads", "All initial technical conditions of roads have been successfully received.");

                return initialTechnicalConditionsOfRoads;
            }
            catch (Exception ex)
            {
                _logger.LogError("TechnicalConditionsOfRoadsAnalysisModule/GetInitialTechnicalConditionsOfRoads", $"Error in getting all initial technical conditions of roads: {ex.Message}");
                return null;
            }
        }

        public Dictionary<int, double>? CalculateChangesTechnicalConditionsOfRoads(Dictionary<int, double> initialTechnicalConditionsOfRoads, Dictionary<int, double> predictedTechnicalConditionsOfRoads)
        {
            try
            {
                _logger.LogInformation("TechnicalConditionsOfRoadsAnalysisModule/CalculateChangesTechnicalConditionsOfRoads", "Calculating technical conditions of roads...");

                Dictionary<int, double> changesTechnicalConditionsOfRoads = initialTechnicalConditionsOfRoads
                    .Join(predictedTechnicalConditionsOfRoads,
                    initialTechnicalConditionOfRoad => initialTechnicalConditionOfRoad.Key,
                    predictedTechnicalConditionOfRoad => predictedTechnicalConditionOfRoad.Key,
                    (initialTechnicalConditionOfRoad, predictedTechnicalConditionOfRoad) =>
                    {
                        double technicalConditionOfRoad = CalculateTechnicalConditionOfRoad(initialTechnicalConditionOfRoad.Value);
                        double changeTechnicalConditionOfRoad = predictedTechnicalConditionOfRoad.Value - technicalConditionOfRoad;

                        return new { key = initialTechnicalConditionOfRoad.Key, value = Math.Clamp(Math.Round(changeTechnicalConditionOfRoad, 1), 0, 5) };
                    })
                    .ToDictionary(changeTechnicalConditionsOfRoads => changeTechnicalConditionsOfRoads.key, changeTechnicalConditionsOfRoads => changeTechnicalConditionsOfRoads.value);

                _logger.LogInformation("TechnicalConditionsOfRoadsAnalysisModule/CalculateChangesTechnicalConditionsOfRoads", "The technical conditions of roads have been successfully calculated.");

                return changesTechnicalConditionsOfRoads;
            }
            catch (Exception ex)
            {
                _logger.LogError("TechnicalConditionsOfRoadsAnalysisModule/CalculateChangesTechnicalConditionsOfRoads", $"Error in calculating the technical conditions of roads: {ex.Message}");
                return null;
            }
        }

        private double CalculateTechnicalConditionOfRoad(double initialTechnicalConditionOfRoad)
        {
            double exp = Math.Exp(-1);
            double technicalConditionOfRoad = initialTechnicalConditionOfRoad * exp;

            return Math.Clamp(Math.Round(technicalConditionOfRoad, 1), 0.1, 5);
        }
    }
}
