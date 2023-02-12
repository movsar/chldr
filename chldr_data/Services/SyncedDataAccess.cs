using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_utils;
using chldr_utils.Services;
using Realms.Sync;

namespace chldr_data.Services
{
    public class SyncedDataAccess : DataAccess, ISyncedDataAccess
    {
        public SyncedDataAccess(IRealmServiceFactory realmServiceFactory, ExceptionHandler exceptionHandler, NetworkService networkService, EntriesRepository<EntryModel> entriesRepository, WordsRepository wordsRepository, PhrasesRepository phrasesRepository, LanguagesRepository languagesRepository, SourcesRepository sourcesRepository) : base(realmServiceFactory, exceptionHandler, networkService, entriesRepository, wordsRepository, phrasesRepository, languagesRepository, sourcesRepository)
        {
        }

        public event Action? DatabaseSynchronized;
        public App App => ((SyncedRealmService)_realmServiceFactory.GetInstance(DataAccessType.Synced)).GetApp();
    }
}
