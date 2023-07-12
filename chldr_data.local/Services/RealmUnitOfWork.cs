using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_utils.Interfaces;
using chldr_utils;
using Realms;
using chldr_data.local.Repositories;
using chldr_utils.Services;

namespace chldr_data.local.Services
{
    public class RealmUnitOfWork : IUnitOfWork
    {
        private RealmChangeSetsRepository _changeSetsRepository;
        private RealmTranslationsRepository _translationsRepository;
        private RealmSourcesRepository _sourcesRepository;
        private RealmUsersRepository _usersRepository;
        private RealmEntriesRepository _entriesRepository;
        private RealmSoundsRepository? _soundsRepository;

        private readonly FileService _fileService;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly string _userId;

        public RealmUnitOfWork(
            ExceptionHandler exceptionHandler,
            FileService fileService,
            string userId
            )
        {
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;
            _userId = userId;
        }

        public RealmTranslationsRepository Translations => _translationsRepository ??= new RealmTranslationsRepository(_exceptionHandler, _fileService, _userId);
        public RealmChangeSetsRepository ChangeSets => _changeSetsRepository ??= new RealmChangeSetsRepository(_exceptionHandler, _fileService, _userId);
        public RealmEntriesRepository Entries => _entriesRepository ??= new RealmEntriesRepository(_exceptionHandler, _fileService, _userId, Translations, Sounds);
        public RealmSourcesRepository Sources => _sourcesRepository ??= new RealmSourcesRepository(_exceptionHandler, _fileService, _userId);
        public RealmUsersRepository Users => _usersRepository ??= new RealmUsersRepository(_exceptionHandler, _fileService, _userId);
        public RealmSoundsRepository Sounds => _soundsRepository ?? new RealmSoundsRepository(_exceptionHandler, _fileService, _userId);

    }
}
