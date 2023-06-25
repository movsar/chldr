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

        private readonly Realm _context;
        private readonly FileService _fileService;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly string _userId;

        public RealmUnitOfWork(
            Realm context, 
            ExceptionHandler exceptionHandler,
            FileService fileService,
            string userId
            )
        {
            _context = context;
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;
            _userId = userId;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public RealmTranslationsRepository Translations => _translationsRepository ??= new RealmTranslationsRepository(_context, _exceptionHandler, _fileService, _userId);
        public RealmChangeSetsRepository ChangeSets => _changeSetsRepository ??= new RealmChangeSetsRepository(_context, _exceptionHandler, _fileService, _userId);
        public RealmEntriesRepository Entries => _entriesRepository ??= new RealmEntriesRepository(_context, _exceptionHandler, _fileService, _userId, Translations, Sounds);
        public RealmSourcesRepository Sources => _sourcesRepository ??= new RealmSourcesRepository(_context, _exceptionHandler, _fileService, _userId);
        public RealmUsersRepository Users => _usersRepository ??= new RealmUsersRepository(_context, _exceptionHandler, _fileService, _userId);
        public RealmSoundsRepository Sounds => _soundsRepository ?? new RealmSoundsRepository(_context, _exceptionHandler, _fileService, _userId);

    }
}
