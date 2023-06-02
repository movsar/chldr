using chldr_data.Interfaces;
using chldr_tools;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using chldr_data.DatabaseObjects.SqlEntities;

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
