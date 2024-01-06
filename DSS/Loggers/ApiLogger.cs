using Microsoft.AspNetCore.Mvc;

namespace DSS.Loggers
{
    public class ApiLogger
    {
        private readonly ILogger<ControllerBase> _logger;

        public ApiLogger(ILogger<ControllerBase> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string module, string message)
        {
            Log(LogLevel.Information, module, message);
        }

        public void LogWarning(string module, string message)
        {
            Log(LogLevel.Warning, module, message);
        }

        public void LogError(string module, string message)
        {
            Log(LogLevel.Error, module, message);
        }

        public void LogCritical(string module, string message)
        {
            Log(LogLevel.Critical, module, message);
        }

        private void Log(LogLevel logLevel, string module, string message)
        {
            string logMessage = $"{DateTime.Now} [{logLevel}] [{module}] {message}";
            _logger.Log(logLevel, logMessage);
        }
    }
}
