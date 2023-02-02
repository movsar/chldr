using chldr_data.Entities;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_data.Search;
using chldr_utils;
using chldr_utils.Models;
using chldr_utils.Services;
using Realms;
using Realms.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Services
{
    public abstract class DataAccess : IDataAccess
    {
        protected readonly ExceptionHandler _exceptionHandler;
        protected readonly NetworkService _networkService;
        protected readonly MainSearchEngine _searchEngine;
        protected readonly SyncedRealmService _realmService;

        public Realm Database => _realmService.GetDatabase();
        public WordsRepository WordsRepository { get; }
        public PhrasesRepository PhrasesRepository { get; }

        public event Action? DatabaseInitialized;
        public event Action<SearchResultModel>? GotNewSearchResult;

        public DataAccess(SyncedRealmService realmService, ExceptionHandler exceptionHandler, NetworkService networkService)
        {
            _exceptionHandler = exceptionHandler;
            _networkService = networkService;
            _realmService = realmService;

            _searchEngine = new MainSearchEngine(this);
            WordsRepository = new WordsRepository(_realmService);
            PhrasesRepository = new PhrasesRepository(_realmService);
        }


        public List<EntryModel> GetRandomEntries()
        {
            var randomizer = new Random();
            var entries = Database.All<Entry>().AsEnumerable()
              .Where(entry => entry.Rate > 0)
              .OrderBy(x => randomizer.Next(0, 70000))
              .Take(50)
              .OrderBy(entry => entry.GetHashCode())
              .Select(entry => EntryModelFactory.CreateEntryModel(entry))
              .ToList();

            return entries;
        }

        public List<EntryModel> GetEntriesOnModeration()
        {
            var entries = Database.All<Entry>().AsEnumerable()
                .Where(entry => entry.Rate < UserModel.EnthusiastRateRange.Lower)
                .Select(entry => EntryModelFactory.CreateEntryModel(entry))
                .ToList();

            return entries;
        }
        public void RequestRandomEntries()
        {
            OnNewResults(new SearchResultModel(GetRandomEntries()));
        }

        public void RequestEntriesOnModeration()
        {
            OnNewResults(new SearchResultModel(GetEntriesOnModeration()));
        }

        public void OnDatabaseInitialized()
        {
            DatabaseInitialized?.Invoke();
        }
        public void OnNewResults(SearchResultModel results)
        {
            GotNewSearchResult?.Invoke(results);
        }
        public List<LanguageModel> GetAllLanguages()
        {
            try
            {
                var languages = Database.All<Language>().AsEnumerable().Select(l => new LanguageModel(l));
                return languages.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<SourceModel> GetAllNamedSources()
        {
            return Database.All<Source>().AsEnumerable().Select(s => new SourceModel(s)).ToList();
        }

        public abstract Task Initialize();
        public abstract Task FindAsync(string inputText, FiltrationFlags filterationFlags);
    }
}
