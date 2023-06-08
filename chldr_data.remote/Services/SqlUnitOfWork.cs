using Microsoft.EntityFrameworkCore.Storage;
using chldr_data.Models;
using Newtonsoft.Json;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.Repositories;

namespace chldr_data.Services
{
    public class SqlUnitOfWork : IUnitOfWork
    {
        private readonly SqlContext _sqlContext;
        private IDbContextTransaction _transaction;

        private IChangeSetsRepository _changeSetsRepository;
        private IWordsRepository _wordsRepository;
        private ITranslationsRepository _translationsRepository;

        public SqlUnitOfWork(SqlContext sqlContext)
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

        public ITranslationsRepository Translations
        {
            get
            {
                if (_translationsRepository == null)
                {
                    _translationsRepository = new SqlTranslationsRepository(_sqlContext);
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
                    _changeSetsRepository = new SqlChangeSetsRepository(_sqlContext);
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
                    _wordsRepository = new SqlWordsRepository(_sqlContext);
                }
                return _wordsRepository;
            }
        }

        public IPhrasesRepository Phrases => throw new NotImplementedException();

        public ILanguagesRepository Languages => throw new NotImplementedException();

        public ISourcesRepository Sources => throw new NotImplementedException();

        public IUsersRepository Users => throw new NotImplementedException();
    }
}
