using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_utils.Interfaces;
using chldr_utils;
using Newtonsoft.Json;
using Realms;

namespace chldr_data.local.Services
{
    public class RealmUnitOfWork : IUnitOfWork
    {
        private IChangeSetsRepository _changeSetsRepository;
        private IWordsRepository _wordsRepository;
        private ITranslationsRepository _translationsRepository;
        private IPhrasesRepository _phrasesRepository;
        private ILanguagesRepository _languagesRepository;
        private ISourcesRepository _sourcesRepository;
        private IUsersRepository _usersRepository;

        private Transaction _transaction;

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

        public void BeginTransaction()
        {
            _transaction = _context.BeginWrite();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }

        public static List<Change> GetChanges<T>(T updated, T existing)
        {
            // This method compares the two dto's and returns the changed properties with their names and values

            var changes = new List<Change>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                // Get currenta and old values, use empty string if they're null
                var newValue = property.GetValue(updated) ?? "";
                var oldValue = property.GetValue(existing) ?? "";

                // ! Serialization allows comparision between complex objects, it might slow down the process though and worth reconsidering
                if (!Equals(JsonConvert.SerializeObject(newValue), JsonConvert.SerializeObject(oldValue)))
                {
                    changes.Add(new Change()
                    {
                        Property = property.Name,
                        OldValue = oldValue,
                        NewValue = newValue,
                    });
                }
            }

            return changes;
        }

        public ITranslationsRepository Translations => _translationsRepository ??= new RealmTranslationsRepository(_context, _exceptionHandler, _graphQLRequestSender);
        public IChangeSetsRepository ChangeSets => _changeSetsRepository ??= new RealmChangeSetsRepository(_context, _exceptionHandler, _graphQLRequestSender);
        public IWordsRepository Words => _wordsRepository ??= new RealmWordsRepository(_context, _exceptionHandler, _graphQLRequestSender);
        public IPhrasesRepository Phrases => _phrasesRepository ??= new RealmPhrasesRepository(_context, _exceptionHandler, _graphQLRequestSender);
        public ILanguagesRepository Languages => _languagesRepository ??= new RealmLanguagesRepository(_context, _exceptionHandler, _graphQLRequestSender);
        public ISourcesRepository Sources => _sourcesRepository ??= new RealmSourcesRepository(_context, _exceptionHandler, _graphQLRequestSender);
        public IUsersRepository Users => _usersRepository ??= new RealmUsersRepository(_context, _exceptionHandler, _graphQLRequestSender);

    }
}
