using chldr_utils.Services;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace chldr_utils
{
    public class ExceptionHandler
    {
        private Logger _fileLogger;
        private ILogger<ExceptionHandler> _consoleLogger;

        public event Action<Exception>? IncomingException;
        public ExceptionHandler(ILogger<ExceptionHandler> consoleLogger, FileService fileService)
        {
            _consoleLogger = consoleLogger;
            _fileLogger = new LoggerConfiguration()
                         .WriteTo.File(Path.Combine(fileService.AppDataDirectory!, "logs", "log.txt"), rollingInterval: RollingInterval.Month)
                         .CreateLogger();
        }
        public void ProcessError(Exception ex)
        {
            string message = Regex.Replace($"{ex.Message} {ex.StackTrace}\r\n", @"\s\s+", "\r\n\t");

            _fileLogger.Error(message);
            _consoleLogger.LogError(message);

            IncomingException?.Invoke(ex);
        }

        public void ProcessDebug(Exception ex)
        {
            string message = Regex.Replace($"{ex.Message} {ex.StackTrace}\r\n", @"\s\s+", "\r\n\t");

            _fileLogger.Debug(message);
            _consoleLogger.LogDebug(message);
        }
    }

}
