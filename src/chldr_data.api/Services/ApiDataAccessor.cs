using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.realm.Repositories;
using chldr_data.Repositories;
using chldr_data.Services;
using chldr_utils;
using chldr_utils.Services;

namespace chldr_data.api.Services
{
    public class ApiDataAccessor : IDataAccessor
    {
        private readonly ExceptionHandler _exceptionHandler;
        private readonly EnvironmentService _environmentService;
        private readonly FileService _fileService;
        private readonly RequestService _requestService;

        public ApiDataAccessor(
            ExceptionHandler exceptionHandler,
            EnvironmentService environmentService,
            FileService fileService,
            RequestService requestService
            )
        {
            _exceptionHandler = exceptionHandler;
            _environmentService = environmentService;
            _fileService = fileService;
            _requestService = requestService;
        }
        public IPronunciationsRepository Sounds => new ApiSoundsRepository(_exceptionHandler, _fileService, _requestService);

        public IChangeSetsRepository ChangeSets => new ApiChangeSetsRepository(_exceptionHandler, _fileService, _requestService);

        public IEntriesRepository Entries => new ApiEntriesRepository(_exceptionHandler, _fileService, _requestService);

        public ITranslationsRepository Translations => new ApiTranslationsRepository(_exceptionHandler, _fileService, _requestService);

        public ISourcesRepository Sources => new ApiSourcesRepository(_exceptionHandler, _fileService, _requestService);

        public IUsersRepository Users => new ApiUsersRepository(_exceptionHandler, _fileService, _requestService);
    }
}
