using domain.Interfaces;
using domain.Services;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System.Text.RegularExpressions;
using Path = System.IO.Path;

namespace domain.Models
{
    public class ExceptionHandler : IExceptionHandler
    {
        private Logger _fileLogger;
        private ILogger<ExceptionHandler>? _consoleLogger;
        public ExceptionHandler(ILogger<ExceptionHandler> consoleLogger, IFileService fileService)
        {
            _consoleLogger = consoleLogger;
            _fileLogger = new LoggerConfiguration()
                         .WriteTo.File(Path.Combine(fileService.AppDataDirectory!, "logs", "log.txt"), rollingInterval: RollingInterval.Month)
                         .CreateLogger();
        }
        public ExceptionHandler(IFileService fileService)
        {
            _fileLogger = new LoggerConfiguration()
                         .WriteTo.File(Path.Combine(fileService.AppDataDirectory!, "logs", "log.txt"), rollingInterval: RollingInterval.Month)
                         .CreateLogger();
        }

        public ExceptionHandler()
        {
        }

        public void LogError(Exception ex, string msg = "")
        {
            string message = Regex.Replace($"{msg} {ex.Message} {ex.StackTrace}\r\n", @"\s\s+", "\r\n\t");
            LogError(message);
        }

        public void LogDebug(string message)
        {
            _fileLogger.Debug(message);
            _consoleLogger?.LogDebug(message);
        }
        public void LogError(string message)
        {
            _fileLogger.Error(message);
            _consoleLogger?.LogError(message);
        }
        public Exception Error(Exception ex)
        {
            return Error(Regex.Replace($"{ex.Message} {ex.StackTrace}\r\n", @"\s\s+", "\r\n\t"));
        }

        public Exception Error(string message)
        {
            LogError(message);
            return new Exception(message);
        }
    }

}
