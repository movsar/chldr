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
        Realm Database { get; }
        EntriesRepository<EntryModel> EntriesRepository { get; }
        PhrasesRepository PhrasesRepository { get; }
        WordsRepository WordsRepository { get; }
        void Initialize();
        List<LanguageModel> GetAllLanguages();
        List<SourceModel> GetAllNamedSources();
    }
}