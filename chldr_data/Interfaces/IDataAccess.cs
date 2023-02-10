using chldr_data.Models;
using chldr_data.Repositories;
using Realms;

namespace chldr_data.Interfaces
{
    public interface IDataAccess
    {
        event Action DatabaseInitialized;
        Realm Database { get; }
        EntriesRepository<EntryModel> EntriesRepository { get; }
        PhrasesRepository PhrasesRepository { get; }
        WordsRepository WordsRepository { get; }
        LanguagesRepository LanguagesRepository { get; }
        Task DatabaseMaintenance();

        void Initialize();
        List<LanguageModel> GetAllLanguages();
        List<SourceModel> GetAllNamedSources();
    }
}