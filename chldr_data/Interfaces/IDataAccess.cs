using chldr_data.Models;
using chldr_data.Repositories;
using chldr_utils.Models;
using MongoDB.Bson;
using Realms;
using Realms.Sync;

namespace chldr_data.Interfaces
{
    public interface IDataAccess
    {
        App App { get; }
        Realm Database { get; }

        PhrasesRepository PhrasesRepository { get; }
        WordsRepository WordsRepository { get; }
        event Action ConnectionInitialized;
        event Action DatabaseInitialized;
        event Action DatabaseSynchronized;
        event Action<SearchResultModel> GotNewSearchResult;

        void RequestRandomEntries();
        List<EntryModel> GetRandomEntries();
        void RequestEntriesOnModeration();
        Task Initialize();
        Task FindAsync(string inputText, FiltrationFlags filterationFlags);
        Task LogInEmailPasswordAsync(string email, string password);
        Task RegisterNewUserAsync(string email, string password);
        Task SendPasswordResetRequestAsync(string email);
        Task UpdatePasswordAsync(string token, string tokenId, string newPassword);
        Task ConfirmUserAsync(string token, string tokenId, string email);
        Task LogOutAsync();
        UserModel GetCurrentUserInfo();
        void OnNewResults(SearchResultModel args);
        List<LanguageModel> GetAllLanguages();
        List<SourceModel> GetAllNamedSources();
    }
}