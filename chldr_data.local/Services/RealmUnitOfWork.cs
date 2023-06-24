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
        private IChangeSetsRepository _changeSetsRepository;
        private ITranslationsRepository _translationsRepository;
        private ISourcesRepository _sourcesRepository;
        private IUsersRepository _usersRepository;
        private IEntriesRepository _entriesRepository;
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

        public ITranslationsRepository Translations => _translationsRepository ??= new RealmTranslationsRepository(_context, _exceptionHandler, _fileService, _userId);
        public IChangeSetsRepository ChangeSets => _changeSetsRepository ??= new RealmChangeSetsRepository(_context, _exceptionHandler, _fileService, _userId);
        public IEntriesRepository Entries => _entriesRepository ??= new RealmEntriesRepository(_context, _exceptionHandler, _fileService, _userId, Translations, Sounds);
        public ISourcesRepository Sources => _sourcesRepository ??= new RealmSourcesRepository(_context, _exceptionHandler, _fileService, _userId);
        public IUsersRepository Users => _usersRepository ??= new RealmUsersRepository(_context, _exceptionHandler, _fileService, _userId);
        public ISoundsRepository Sounds => _soundsRepository ?? new RealmSoundsRepository(_context, _exceptionHandler, _fileService, _userId);

    }
}
