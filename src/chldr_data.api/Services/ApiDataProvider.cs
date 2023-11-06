using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_utils;
using chldr_utils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.api.Services
{
    public class ApiDataProvider : IDataProvider
    {
        private readonly RequestService _requestService;
        private readonly EnvironmentService _environmentService;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly FileService _fileService;

        public ApiDataProvider(
            RequestService requestService,
            EnvironmentService environmentService,
            ExceptionHandler exceptionHandler,
            FileService fileService)
        {
            _requestService = requestService;
            _environmentService = environmentService;
            _exceptionHandler = exceptionHandler;
            _fileService = fileService;
        }

        public bool IsInitialized { get; set; } = true;

        public event Action? DatabaseInitialized;

        public IDataAccessor CreateUnitOfWork(string? userId = null)
        {
            return new ApiDataAccessor(_exceptionHandler, _environmentService, _fileService, _requestService);
        }

        public void Initialize()
        {
            DatabaseInitialized?.Invoke();
        }

        public void TruncateDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
