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

        public Dictionary<string, double>? GetInitialTechnicalConditionsOfRoads(InputDataViewModel inputData)
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

                Dictionary<string, double>? initialTechnicalConditionsOfRoads = new();

                Dictionary<string, int> months = new()
                {
                    ["Январь"] = 1,
                    ["Февраль"] = 2,
                    ["Март"] = 3,
                    ["Апрель"] = 4,
                    ["Май"] = 5,
                    ["Июнь"] = 6,
                    ["Июль"] = 7,
                    ["Август"] = 8,
                    ["Сентябрь"] = 9,
                    ["Октябрь"] = 10,
                    ["Ноябрь"] = 11,
                    ["Декабрь"] = 12
                };

                int initialYear = inputData.InitialYear;
                int initialMonth = months[inputData.InitialMonth] - 1;

                if (initialMonth == 0)
                {
                    initialYear -= 1;
                    initialMonth = 12;
                }

                technicalConditionsOfRoads = technicalConditionsOfRoads.Where(tc => tc.Year > initialYear ||
                (tc.Year == initialYear && months[tc.Month] >= initialMonth));

                if (technicalConditionsOfRoads.Count() == 0)
                {
                    _logger.LogWarning("TechnicalConditionsOfRoadsAnalysisModule/GetInitialTechnicalConditionsOfRoads", "The technical conditions of roads were not found.");
                    return null;
                }

                string roadNumber = technicalConditionsOfRoads.First().Road.Number;
                int month = months[technicalConditionsOfRoads.First().Month];
                double initialTechnicalConditionOfRoad = (double)technicalConditionsOfRoads.First().TechnicalCondition;

                foreach (var technicalConditionOfRoad in technicalConditionsOfRoads)
                {
                    if (roadNumber != technicalConditionOfRoad.Road.Number)
                    {
                        initialTechnicalConditionsOfRoads.Add(roadNumber, initialTechnicalConditionOfRoad);

                        roadNumber = technicalConditionOfRoad.Road.Number;
                        month = months[technicalConditionOfRoad.Month];
                        initialTechnicalConditionOfRoad = (double)technicalConditionOfRoad.TechnicalCondition;
                    }
                    else
                    {
                        if (month < months[technicalConditionOfRoad.Month])
                        {
                            month = months[technicalConditionOfRoad.Month];
                            initialTechnicalConditionOfRoad = (double)technicalConditionOfRoad.TechnicalCondition;
                        }
                    }
                }

                initialTechnicalConditionsOfRoads.Add(roadNumber, initialTechnicalConditionOfRoad);

                _logger.LogInformation("TechnicalConditionsOfRoadsAnalysisModule/GetInitialTechnicalConditionsOfRoads", "All initial technical conditions of roads have been successfully received.");

                return initialTechnicalConditionsOfRoads;
            }
            catch (Exception ex)
            {
                _logger.LogError("TechnicalConditionsOfRoadsAnalysisModule/GetInitialTechnicalConditionsOfRoads", $"Error in getting all initial technical conditions of roads: {ex.Message}");
                return null;
            }
        }

        public Dictionary<string, double>? CalculateChangesTechnicalConditionsOfRoads(Dictionary<string, double> initialTechnicalConditionsOfRoads, Dictionary<string, double> predictedTechnicalConditionsOfRoads)
        {
            try
            {
                _logger.LogInformation("TechnicalConditionsOfRoadsAnalysisModule/CalculateChangesTechnicalConditionsOfRoads", "Calculating technical conditions of roads...");

                Dictionary<string, double> changesTechnicalConditionsOfRoads = new();

                foreach (var initialTechnicalConditionOfRoad in initialTechnicalConditionsOfRoads)
                {
                    foreach (var predictedTechnicalConditionOfRoad in predictedTechnicalConditionsOfRoads)
                    {
                        if (initialTechnicalConditionOfRoad.Key == predictedTechnicalConditionOfRoad.Key)
                        {
                            double technicalConditionOfRoad = CalculateTechnicalConditionOfRoad(initialTechnicalConditionOfRoad.Value);
                            double changeTechnicalConditionOfRoad = predictedTechnicalConditionOfRoad.Value - technicalConditionOfRoad;

                            if (changeTechnicalConditionOfRoad > 5)
                                changeTechnicalConditionOfRoad = 5;
                            if (changeTechnicalConditionOfRoad < 0.1)
                                changeTechnicalConditionOfRoad = 0;

                            changesTechnicalConditionsOfRoads.Add(initialTechnicalConditionOfRoad.Key, Math.Round(changeTechnicalConditionOfRoad, 1));
                        }
                    }
                }

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

            if (technicalConditionOfRoad > 5)
                technicalConditionOfRoad = 5;
            if (technicalConditionOfRoad < 0.1)
                technicalConditionOfRoad = 0.1;

            return Math.Round(technicalConditionOfRoad, 1);
        }
    }
}
