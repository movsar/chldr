using domain.Interfaces;
using domain.Interfaces.Repositories;
using realm_dl.Repositories;

namespace realm_dl.Services
{
    public class RealmDataAccessor : IDataAccessor
    {
        private RealmChangeSetsRepository _changeSetsRepository;
        private RealmTranslationsRepository _translationsRepository;
        private RealmSourcesRepository _sourcesRepository;
        private RealmUsersRepository _usersRepository;
        private RealmEntriesRepository _entriesRepository;
        private RealmSoundsRepository _soundsRepository;

        private readonly IFileService _fileService;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly string? _userId;

        public RealmDataAccessor(
            IExceptionHandler exceptionHandler,
            IFileService fileService,
            string? userId
            )
        {
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;
            _userId = userId;
        }

        public ITranslationsRepository Translations => _translationsRepository ??= new RealmTranslationsRepository(_exceptionHandler, _fileService, _userId!);
        public IChangeSetsRepository ChangeSets => _changeSetsRepository ??= new RealmChangeSetsRepository(_exceptionHandler, _fileService, _userId!);
        public IEntriesRepository Entries => _entriesRepository ??= new RealmEntriesRepository(_exceptionHandler, _fileService, _userId!);
        public ISourcesRepository Sources => _sourcesRepository ??= new RealmSourcesRepository(_exceptionHandler, _fileService, _userId!);
        public IUsersRepository Users => _usersRepository ??= new RealmUsersRepository(_exceptionHandler, _fileService, _userId!);
        public ISoundsRepository Sounds => _soundsRepository ?? new RealmSoundsRepository(_exceptionHandler, _fileService, _userId!);

    }
}
