using Microsoft.EntityFrameworkCore.Storage;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.Repositories;
using chldr_utils.Services;

namespace chldr_data.Services
{
    public class SqlUnitOfWork : ISqlUnitOfWork
    {
        private readonly SqlContext _context;
        private readonly string _userId;
        private IDbContextTransaction _transaction;

        private IChangeSetsRepository _changeSetsRepository;
        private ITranslationsRepository _translationsRepository;
        private SqlEntriesRepository _entriesRepository;
        private SqlSourcesRepository _sourcesRepository;
        private SqlUsersRepository _usersRepository;
        private SqlSoundsRepository _soundsRepository;
        private FileService _fileService;

        public SqlUnitOfWork(SqlContext sqlContext, FileService fileService, string userId)
        {
            _context = sqlContext;
            _userId = userId;
            _fileService = fileService;
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

        public ITranslationsRepository Translations => _translationsRepository ??= new SqlTranslationsRepository(_context, _userId);
        public IChangeSetsRepository ChangeSets => _changeSetsRepository ??= new SqlChangeSetsRepository(_context, _userId);
        public IEntriesRepository Entries => _entriesRepository ??= new SqlEntriesRepository(_context, _fileService, _userId, Translations);
        public ISourcesRepository Sources => _sourcesRepository ??= new SqlSourcesRepository(_context, _userId);
        public IUsersRepository Users => _usersRepository ??= new SqlUsersRepository(_context, _userId);
        public ISoundsRepository Sounds => _soundsRepository ?? new SqlSoundsRepository(_context, _userId);
    }
}
