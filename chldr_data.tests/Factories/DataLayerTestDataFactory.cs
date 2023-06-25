using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using GraphQL.Validation;

namespace chldr_data.tests.Services
{
    internal static class DataLayerTestDataFactory
    {
        private static readonly FileService _fileService;
        private static readonly ExceptionHandler _exceptionHandler;
        private static readonly EnvironmentService _environmentService;

        static DataLayerTestDataFactory()
        {
            _fileService = new FileService(AppContext.BaseDirectory);
            _exceptionHandler = new ExceptionHandler(_fileService);
            _environmentService = new EnvironmentService(chldr_shared.Enums.Platforms.Windows, true);
        }
    }
}
