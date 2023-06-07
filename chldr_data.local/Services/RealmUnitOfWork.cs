using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_tools;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Realms;

namespace chldr_data.local.Services
{
    public class RealmUnitOfWork : IUnitOfWork
    {
        private IChangeSetsRepository _changeSetsRepository;
        private IWordsRepository _wordsRepository;
        private ITranslationsRepository _translationsRepository;
        private Transaction _transaction;
        private readonly Realm _context;

        public RealmUnitOfWork(Realm context)
        {
            _context = context;
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
            if (_transaction != null)
            {
                _transaction.Rollback();
            }
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

        public ITranslationsRepository Translations
        {
            get
            {
                if (_translationsRepository == null)
                {
                    _translationsRepository = new RealmTranslationsRepository(_context);
                }
                return _translationsRepository;
            }
        }


        public IChangeSetsRepository ChangeSets
        {
            get
            {
                if (_changeSetsRepository == null)
                {
                    _changeSetsRepository = new RealmChangeSetsRepository(_context);
                }
                return _changeSetsRepository;
            }
        }

        public IWordsRepository Words
        {
            get
            {
                if (_wordsRepository == null)
                {
                    _wordsRepository = new RealmWordsRepository(_context);
                }
                return _wordsRepository;
            }
        }
    }
}
