﻿using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_utils.Interfaces;
using chldr_utils;
using Realms;
using chldr_data.local.Repositories;

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
        private readonly ExceptionHandler _exceptionHandler;
        private readonly IGraphQLRequestSender _graphQLRequestSender;

        public RealmUnitOfWork(
            Realm context, 
            ExceptionHandler exceptionHandler, 
            IGraphQLRequestSender graphQLRequestSender)
        {
            _context = context;
            _exceptionHandler = exceptionHandler;
            _graphQLRequestSender = graphQLRequestSender;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public ITranslationsRepository Translations => _translationsRepository ??= new RealmTranslationsRepository(_context, _exceptionHandler);
        public IChangeSetsRepository ChangeSets => _changeSetsRepository ??= new RealmChangeSetsRepository(_context, _exceptionHandler);
        public IEntriesRepository Entries => _entriesRepository ??= new RealmEntriesRepository(_context, _exceptionHandler);
        public ISourcesRepository Sources => _sourcesRepository ??= new RealmSourcesRepository(_context, _exceptionHandler);
        public IUsersRepository Users => _usersRepository ??= new RealmUsersRepository(_context, _exceptionHandler);
        public ISoundsRepository Sounds => _soundsRepository ?? new RealmSoundsRepository(_context, _exceptionHandler);

    }
}
