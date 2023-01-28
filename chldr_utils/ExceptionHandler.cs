using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_utils
{
    public class ExceptionHandler
    {
        private ILogger<ExceptionHandler> _logger;

        public event Action<Exception>? IncomingException;
        public ExceptionHandler(ILogger<ExceptionHandler> logger)
        {
            _logger = logger;
        }
        public void ProcessError(Exception ex)
        {
            _logger.LogError($"{ex.Message} {ex.GetType()}");
            _logger.LogError(ex.StackTrace);

            IncomingException?.Invoke(ex);
        }
    }
}
