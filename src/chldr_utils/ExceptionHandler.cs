using chldr_utils.Services;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System.Text.RegularExpressions;

namespace chldr_utils
{
    public class ExceptionHandler
    {
        private Logger _fileLogger;
        private ILogger<ExceptionHandler>? _consoleLogger;

        public event Action<Exception>? IncomingException;
        public ExceptionHandler(ILogger<ExceptionHandler> consoleLogger, FileService fileService)
        {
            _consoleLogger = consoleLogger;
            _fileLogger = new LoggerConfiguration()
                         .WriteTo.File(Path.Combine(fileService.AppDataDirectory!, "logs", "log.txt"), rollingInterval: RollingInterval.Month)
                         .CreateLogger();
        }
        public ExceptionHandler(FileService fileService)
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
