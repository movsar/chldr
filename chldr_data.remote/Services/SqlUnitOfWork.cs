﻿using Microsoft.EntityFrameworkCore.Storage;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.Repositories;
using chldr_utils.Services;
using chldr_utils;
using chldr_data.remote.SqlEntities;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.Services
{
    public class SqlUnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly string? _userId;
        private readonly DbContextOptions<SqlContext> _dbConfig;
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

        public SqlUnitOfWork(DbContextOptions<SqlContext> dbConfig, FileService fileService, ExceptionHandler exceptionHandler, string userId)
        {
            _dbConfig = dbConfig;
            _context = new SqlContext(_dbConfig);
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

        public ITranslationsRepository Translations => _translationsRepository ??= new SqlTranslationsRepository(_dbConfig, _fileService, _userId);
        public IChangeSetsRepository ChangeSets => _changeSetsRepository ??= new SqlChangeSetsRepository(_dbConfig, _fileService, _userId);
        public IEntriesRepository Entries => _entriesRepository ??= new SqlEntriesRepository(_dbConfig, _fileService, _exceptionHandler, Translations, Sounds, _userId);
        public ISourcesRepository Sources => _sourcesRepository ??= new SqlSourcesRepository(_dbConfig, _fileService, _userId);
        public IUsersRepository Users => _usersRepository ??= new SqlUsersRepository(_dbConfig, _fileService, _userId);
        public ISoundsRepository Sounds => _soundsRepository ?? new SqlSoundsRepository(_dbConfig, _fileService, _userId);
        public ITokensRepository Tokens => _tokensRepository ?? new SqlTokensRepository(_dbConfig, _fileService, _userId);
    }
}
