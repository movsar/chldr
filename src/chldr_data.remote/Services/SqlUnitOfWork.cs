using Microsoft.EntityFrameworkCore.Storage;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.Repositories;
using chldr_utils.Services;
using chldr_utils;
using chldr_data.remote.SqlEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using chldr_data.Resources.Localizations;

namespace chldr_data.Services
{
    public class SqlUnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly string? _userId;
        private readonly DbContextOptions<SqlContext> _dbConfig;
        private readonly SqlContext _context;
        private readonly FileService _fileService;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly IStringLocalizer<AppLocalizations> _localizer;
        private readonly UserManager<SqlUser> _userManager;
        private readonly EmailService _emailService;
        private readonly SignInManager<SqlUser> _signInManager;
        private IDbContextTransaction _transaction;

        private SqlChangeSetsRepository _changeSetsRepository;
        private SqlTranslationsRepository _translationsRepository;
        private SqlEntriesRepository _entriesRepository;
        private SqlSourcesRepository _sourcesRepository;
        private SqlUsersRepository _usersRepository;
        private SqlPronunciationsRepository _soundsRepository;
        public SqlUnitOfWork(
            DbContextOptions<SqlContext> dbConfig,
            FileService fileService,
            ExceptionHandler exceptionHandler,
            UserManager<SqlUser> userManager,
            SignInManager<SqlUser> signInManager,
            EmailService emailService,
            IStringLocalizer<AppLocalizations> localizer,
            string userId)
        {
            _dbConfig = dbConfig;
            _context = new SqlContext(_dbConfig);
            _fileService = fileService;
            _userId = userId;
            _exceptionHandler = exceptionHandler;
            _localizer = localizer;
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            if (_transaction != null && _transaction.GetDbTransaction().Connection != null)
            {
                _transaction.Rollback();
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }

        public ITranslationsRepository Translations => _translationsRepository ??= new SqlTranslationsRepository(_dbConfig, _fileService, _userId);
        public IChangeSetsRepository ChangeSets => _changeSetsRepository ??= new SqlChangeSetsRepository(_dbConfig, _fileService, _userId);
        public IEntriesRepository Entries => _entriesRepository ??= new SqlEntriesRepository(_dbConfig, _fileService, _exceptionHandler, Translations, Sounds, _userId);
        public ISourcesRepository Sources => _sourcesRepository ??= new SqlSourcesRepository(_dbConfig, _fileService, _userId);
        public IUsersRepository Users => _usersRepository ??= new SqlUsersRepository(_dbConfig, _fileService, _userManager, _signInManager, _emailService, _localizer, _userId);
        public IPronunciationsRepository Sounds => _soundsRepository ?? new SqlPronunciationsRepository(_dbConfig, _fileService, _userId);
    }
}
