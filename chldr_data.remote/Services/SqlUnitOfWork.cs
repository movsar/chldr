using Microsoft.EntityFrameworkCore.Storage;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.Repositories;
using chldr_utils.Services;
using chldr_utils;

namespace chldr_data.Services
{
    public class SqlUnitOfWork : IUnitOfWork
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

        internal SqlTranslationsRepository Translations => _translationsRepository ??= new SqlTranslationsRepository(_context, _fileService, _userId);
        internal SqlChangeSetsRepository ChangeSets => _changeSetsRepository ??= new SqlChangeSetsRepository(_context, _fileService, _userId);
        internal SqlEntriesRepository Entries => _entriesRepository ??= new SqlEntriesRepository(_context, _fileService, _exceptionHandler, Translations, Sounds, _userId);
        internal SqlSourcesRepository Sources => _sourcesRepository ??= new SqlSourcesRepository(_context, _fileService, _userId);
        internal SqlUsersRepository Users => _usersRepository ??= new SqlUsersRepository(_context, _fileService, _userId);
        internal SqlSoundsRepository Sounds => _soundsRepository ?? new SqlSoundsRepository(_context, _fileService, _userId);
    }
}
