﻿using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.Repositories;
using chldr_utils;
using chldr_data.realm.Repositories;
using chldr_utils.Services;
using chldr_data.Services;

namespace chldr_data.realm.Services
{
    public class RealmDataAccessor : IDataAccessor
    {
        private RealmChangeSetsRepository _changeSetsRepository;
        private RealmTranslationsRepository _translationsRepository;
        private RealmSourcesRepository _sourcesRepository;
        private RealmUsersRepository _usersRepository;
        private RealmEntriesRepository _entriesRepository;
        private RealmSoundsRepository? _soundsRepository;

        private readonly FileService _fileService;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly string? _userId;

        public RealmDataAccessor(
            ExceptionHandler exceptionHandler,
            FileService fileService,
            string? userId
            )
        {
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;
            _userId = userId;
        }

        public ITranslationsRepository Translations => _translationsRepository ??= new RealmTranslationsRepository(_exceptionHandler, _fileService, _userId);
        public IChangeSetsRepository ChangeSets => _changeSetsRepository ??= new RealmChangeSetsRepository(_exceptionHandler, _fileService, _userId);
        public IEntriesRepository Entries => _entriesRepository ??= new RealmEntriesRepository(_exceptionHandler, _fileService, _userId);
        public ISourcesRepository Sources => _sourcesRepository ??= new RealmSourcesRepository(_exceptionHandler, _fileService, _userId);
        public IUsersRepository Users => _usersRepository ??= new RealmUsersRepository(_exceptionHandler, _fileService, _userId);
        public IPronunciationsRepository Sounds => _soundsRepository ?? new RealmSoundsRepository(_exceptionHandler, _fileService, _userId);

    }
}
