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
