using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace DSS.Modules
{
    public class DataAnalysisModule
    {
        private readonly TechnicalConditionsOfRoadsApiController _technicalConditionsOfRoadsApi;
        private readonly ApiLogger _logger;

        public DataAnalysisModule(ApplicationContext context, ILogger<ApiController> logger)
        {
            _technicalConditionsOfRoadsApi = new TechnicalConditionsOfRoadsApiController(context, logger);
            _logger = new ApiLogger(logger);
        }

        private readonly string _scriptsFolderPath = @"Modules\PythonModule";
        private readonly string _interpreterPath = @"Modules\PythonModule\venv\Scripts\python.exe";

        public bool ComparePredictionMethods()
        {
            try
            {
                _logger.LogInformation("DataAnalysisModule/ComparePredictionMethods", "Reading all technical conditions of roads...");

                var result = _technicalConditionsOfRoadsApi.Get();
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("DataAnalysisModule/ComparePredictionMethods", "Error on the API side of the controller.");
                    return false;
                }

                string dataPath = Path.Combine(_scriptsFolderPath, "data.json");
                File.WriteAllText(dataPath, value.ToString());

                _logger.LogInformation("DataAnalysisModule/ComparePredictionMethods", "All technical conditions of roads have been successfully read.");

                _logger.LogInformation("DataAnalysisModule/ComparePredictionMethods", "Comparing prediction methods...");

                string scriptPath = Path.Combine(_scriptsFolderPath, "ComparePredictionMethods.py");
                string? response = ExecutePythonScript(scriptPath, _interpreterPath);

                if (response == null)
                {
                    return false;
                }

                _logger.LogInformation("DataAnalysisModule/ComparePredictionMethods", "Prediction methods have been successfully compared.");
                _logger.LogInformation("DataAnalysisModule/ComparePredictionMethods", $"Result:\n{response}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("DataAnalysisModule/ComparePredictionMethods", $"Error in comparing prediction methods: {ex.Message}");
                return false;
            }
        }

        public Dictionary<int, double>? PredictTechnicalConditionsOfRoads()
        {
            try
            {
                _logger.LogInformation("DataAnalysisModule/PredictTechnicalConditionsOfRoads", "Reading all technical conditions of roads...");

                var result = _technicalConditionsOfRoadsApi.Get();
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("DataAnalysisModule/PredictTechnicalConditionsOfRoads", "Error on the API side of the controller.");
                    return null;
                }

                string dataPath = Path.Combine(_scriptsFolderPath, "data.json");
                File.WriteAllText(dataPath, value.ToString());

                _logger.LogInformation("DataAnalysisModule/PredictTechnicalConditionsOfRoads", "All technical conditions of roads have been successfully read.");

                _logger.LogInformation("DataAnalysisModule/PredictTechnicalConditionsOfRoads", "Predicting the technical conditions of roads...");

                string scriptPath = Path.Combine(_scriptsFolderPath, "PredictTechnicalConditionsOfRoads.py");
                string? response = ExecutePythonScript(scriptPath, _interpreterPath);

                if (response == null)
                {
                    return null;
                }

                Dictionary<int, double> predictions = JsonConvert.DeserializeObject<Dictionary<int, double>>(response);

                foreach (var prediction in predictions)
                {
                    double predictedTechnicalConditionOfRoad = Math.Round(prediction.Value, 1);
                    predictedTechnicalConditionOfRoad = Math.Clamp(predictedTechnicalConditionOfRoad, 0.1, 5);
                    predictions[prediction.Key] = predictedTechnicalConditionOfRoad;
                }

                _logger.LogInformation("DataAnalysisModule/PredictTechnicalConditionsOfRoads", "Technical conditions of roads have been successfully predicted.");

                return predictions;
            }
            catch (Exception ex)
            {
                _logger.LogError("DataAnalysisModule/PredictTechnicalConditionsOfRoads", $"Error in predicting technical conditions of roads: {ex.Message}");
                return null;
            }
        }

        private string? ExecutePythonScript(string scriptPath, string interpreterPath)
        {
            try
            {
                _logger.LogInformation("DataAnalysisModule/ExecutePythonScript", "Executing a Python script...");

                if (!File.Exists(scriptPath))
                {
                    _logger.LogWarning("DataAnalysisModule/ExecutePythonScript", "The Python script was not found.");
                    return null;
                }

                if (!File.Exists(interpreterPath))
                {
                    _logger.LogWarning("DataAnalysisModule/ExecutePythonScript", "The Python interpreter was not found.");
                    return null;
                }

                string output = "";

                using (Process process = new Process())
                {
                    process.StartInfo.FileName = interpreterPath;
                    process.StartInfo.Arguments = scriptPath;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.StandardOutputEncoding = Encoding.UTF8;

                    process.Start();

                    output = process.StandardOutput.ReadToEnd();

                    process.WaitForExit();
                }

                _logger.LogInformation("DataAnalysisModule/ExecutePythonScript", "The Python script has been successfully executed.");

                return output;
            }
            catch (Exception ex)
            {
                _logger.LogError("DataAnalysisModule/ExecutePythonScript", $"Error in executing a Python script: {ex.Message}");
                return null;
            }
        }
    }
}
