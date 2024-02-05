using DSS.Controllers.ApiControllers;
using DSS.Loggers;
using DSS.Models;
using Microsoft.AspNetCore.Mvc;
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

        private readonly string scriptsFolderPath = @"Modules\DataAnalysisModule";
        private readonly string interpreterPath = @"Modules\DataAnalysisModule\.venv\Scripts\python.exe";

        public bool ComparePredictionMethods()
        {
            try
            {
                _logger.LogInformation("DataAnalysisModule", "Comparing prediction methods...");
                _logger.LogInformation("DataAnalysisModule", "Reading all technical conditions of roads...");

                var result = _technicalConditionsOfRoadsApi.Get();
                var statusCode = ((ObjectResult)result).StatusCode;
                var value = ((ObjectResult)result).Value;

                if (statusCode != 200)
                {
                    _logger.LogWarning("DataAnalysisModule", "Error on the API side of the controller");
                    return false;
                }

                string dataPath = Path.Combine(scriptsFolderPath, "data.json");
                File.WriteAllText(dataPath, value.ToString());

                _logger.LogInformation("DataAnalysisModule", "All technical conditions of roads have been successfully read.");

                string scriptPath = Path.Combine(scriptsFolderPath, "ComparePredictionMethods.py");
                string? response = ExecutePythonScript(scriptPath, interpreterPath);

                File.Delete(dataPath);

                if (response == null)
                {
                    return false;
                }

                _logger.LogInformation("DataAnalysisModule", "Prediction methods have been successfully compared.");
                _logger.LogInformation("DataAnalysisModule", $"Result:\n{response}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("DataAnalysisModule", $"Error in comparing prediction methods: {ex.Message}");
                return false;
            }
        }

        private string? ExecutePythonScript(string scriptPath, string interpreterPath)
        {
            try
            {
                _logger.LogInformation("DataAnalysisModule", "Executing a Python script...");

                if (!File.Exists(scriptPath))
                {
                    _logger.LogWarning("DataAnalysisModule", "The Python script was not found.");
                    return null;
                }

                if (!File.Exists(interpreterPath))
                {
                    _logger.LogWarning("DataAnalysisModule", "The Python interpreter was not found.");
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

                _logger.LogInformation("DataAnalysisModule", "The Python script has been successfully executed.");

                return output;
            }
            catch (Exception ex)
            {
                _logger.LogError("DataAnalysisModule", $"Error in executing a Python script: {ex.Message}");
                return null;
            }
        }
    }
}
