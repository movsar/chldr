using chldr_data.Entities;
using chldr_data.Factories;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_data.Services;
using chldr_utils.Models;
using MongoDB.Bson;
using Realms;
using Realms.Sync;

namespace chldr_data.Interfaces
{
    public interface IDataAccess
    {

        event Action DatabaseInitialized;
        event Action<SearchResultModel> GotNewSearchResult;

        Realm Database { get; }
        PhrasesRepository PhrasesRepository { get; }
        WordsRepository WordsRepository { get; }
        Task Initialize();
        Task FindAsync(string inputText, FiltrationFlags filterationFlags);
        void RequestRandomEntries();
        void RequestEntriesOnModeration();
        void OnNewResults(SearchResultModel args);
        List<EntryModel> GetRandomEntries();
        List<LanguageModel> GetAllLanguages();
        List<SourceModel> GetAllNamedSources();
    }
}