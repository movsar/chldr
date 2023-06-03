using chldr_tools;
using Microsoft.EntityFrameworkCore.Storage;
using chldr_data.Models;
using Newtonsoft.Json;

namespace chldr_data.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private readonly SqlContext _sqlContext;
        private IDbContextTransaction _transaction;

        private ChangeSetsRepository _changeSetsRepository;
        private WordsRepository _wordsRepository;
        private TranslationsRepository _translationsRepository;

        public UnitOfWork(SqlContext sqlContext)
        {
            _sqlContext = sqlContext;
        }

        public void BeginTransaction()
        {
            _transaction = _sqlContext.Database.BeginTransaction();
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
            _sqlContext.Dispose();
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

        public TranslationsRepository Translations
        {
            get
            {
                if (_translationsRepository == null)
                {
                    _translationsRepository = new TranslationsRepository(_sqlContext);
                }
                return _translationsRepository;
            }
        }


        public ChangeSetsRepository ChangeSets
        {
            get
            {
                if (_changeSetsRepository == null)
                {
                    _changeSetsRepository = new ChangeSetsRepository(_sqlContext);
                }
                return _changeSetsRepository;
            }
        }

        public WordsRepository Words
        {
            get
            {
                if (_wordsRepository == null)
                {
                    _wordsRepository = new WordsRepository(_sqlContext);
                }
                return _wordsRepository;
            }
        }
    }
}
