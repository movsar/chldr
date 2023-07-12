using Microsoft.EntityFrameworkCore.Storage;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.Repositories;
using chldr_utils.Services;
using chldr_utils;
using chldr_data.remote.SqlEntities;

namespace chldr_data.Services
{
    public class SqlUnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly string _userId;
        private readonly SqlContext _context;
        private readonly FileService _fileService;
        private readonly ExceptionHandler _exceptionHandler;
        private IDbContextTransaction _transaction;

        private SqlChangeSetsRepository _changeSetsRepository;
        private SqlTranslationsRepository _translationsRepository;
        private SqlEntriesRepository _entriesRepository;
        private SqlSourcesRepository _sourcesRepository;
        private SqlUsersRepository _usersRepository;
        private SqlSoundsRepository _soundsRepository;
        private SqlTokensRepository? _tokensRepository;

        public SqlUnitOfWork(SqlContext sqlContext, FileService fileService, ExceptionHandler exceptionHandler, string userId)
        {
            _context = sqlContext;
            _fileService = fileService;
            _userId = userId;
            _exceptionHandler = exceptionHandler;
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

        public SqlTranslationsRepository Translations => _translationsRepository ??= new SqlTranslationsRepository(_context, _fileService, _userId);
        public SqlChangeSetsRepository ChangeSets => _changeSetsRepository ??= new SqlChangeSetsRepository(_context, _fileService, _userId);
        public SqlEntriesRepository Entries => _entriesRepository ??= new SqlEntriesRepository(_context, _fileService, _exceptionHandler, Translations, Sounds, _userId);
        public SqlSourcesRepository Sources => _sourcesRepository ??= new SqlSourcesRepository(_context, _fileService, _userId);
        public SqlUsersRepository Users => _usersRepository ??= new SqlUsersRepository(_context, _fileService, _userId);
        public SqlSoundsRepository Sounds => _soundsRepository ?? new SqlSoundsRepository(_context, _fileService, _userId);
        public SqlTokensRepository Tokens => _tokensRepository ?? new SqlTokensRepository(_context, _fileService, _userId);
    }
}
