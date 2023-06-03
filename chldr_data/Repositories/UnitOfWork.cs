using chldr_data.Interfaces;
using chldr_tools;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Models;
using chldr_data.DatabaseObjects.Interfaces;
using System.Security.Claims;
using Newtonsoft.Json;

namespace chldr_data.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private readonly SqlContext _sqlContext;
        private IDbContextTransaction _transaction;

        private IChangeSetsRepository _changeSetsRepository;
        private IWordsRepository _wordsRepository;
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
            _transaction.Rollback();
        }

        public async Task SaveChangesAsync()
        {
            await _sqlContext.SaveChangesAsync();
        }

        public void SaveChanges()
        {
            _sqlContext.SaveChanges();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _sqlContext.Dispose();
        }
        public List<Change> GetChanges<T>(T updated, T existing)
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

        private void SetPropertyValue(object obj, string propertyName, object value)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value);
            }
        }

        public void ApplyChanges<T>(string entityId, List<Change> changes) where T : class, IEntity
        {
            // Using this method, instead of updating the whole database entity, we can just update its particular, changed fields
            
            var sqlEntity = _sqlContext.Find<T>(entityId);
            if (sqlEntity == null)
            {
                throw new NullReferenceException();
            }

            foreach (var change in changes)
            {
                SetPropertyValue(sqlEntity, change.Property, change.NewValue);
            }
        }

        public ITranslationsRepository Translations
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


        public IChangeSetsRepository ChangeSets
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

        public IWordsRepository Words
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
