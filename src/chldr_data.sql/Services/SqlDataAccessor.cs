﻿using Microsoft.EntityFrameworkCore.Storage;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.sql.Services;
using chldr_data.sql.Repositories;
using chldr_utils.Services;
using chldr_utils;

namespace chldr_data.Services
{
    public class SqlDataAccessor : IDataAccessor, IDisposable
    {
        private readonly string? _userId;
        private readonly SqlContext _context;
        private readonly FileService _fileService;
        private readonly ExceptionHandler _exceptionHandler;
        private IDbContextTransaction _transaction;

        private SqlChangeSetsRepository _changeSetsRepository;
        private SqlTranslationsRepository _translationsRepository;
        private SqlEntriesRepository _entriesRepository;
        private SqlSourcesRepository _sourcesRepository;
        private SqlUsersRepository _usersRepository;
        private SqlPronunciationsRepository _soundsRepository;
        public SqlDataAccessor(
            SqlContext context,
            FileService fileService,
            ExceptionHandler exceptionHandler,
            string userId)
        {
            _context = context;
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

        public ITranslationsRepository Translations => _translationsRepository ??= new SqlTranslationsRepository(_context, _fileService, _userId);
        public IChangeSetsRepository ChangeSets => _changeSetsRepository ??= new SqlChangeSetsRepository(_context, _fileService, _userId);
        public IEntriesRepository Entries => _entriesRepository ??= new SqlEntriesRepository(_context, _fileService, _exceptionHandler, Translations, Sounds, _userId);
        public ISourcesRepository Sources => _sourcesRepository ??= new SqlSourcesRepository(_context, _fileService, _userId);
        public IUsersRepository Users => _usersRepository ??= new SqlUsersRepository(_context, _fileService, _userId);
        public IPronunciationsRepository Sounds => _soundsRepository ?? new SqlPronunciationsRepository(_context, _fileService, _userId);
    }
}