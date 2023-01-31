using chldr_data.Models;
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

        event Action ConnectionInitialized;
        event Action DatabaseInitialized;
        event Action DatabaseSynchronized;
        event Action<SearchResultModel> GotNewSearchResult;

        WordModel GetWordById(ObjectId entityId);
        PhraseModel GetPhraseById(ObjectId entityId);
        void RequestRandomEntries();
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
        PhraseModel? GetPhraseByEntryId(ObjectId entryId);
        WordModel? GetWordByEntryId(ObjectId entryId);
        EntryModel? GetEntryById(ObjectId entryId);
        void OnNewResults(SearchResultModel args);
        List<LanguageModel> GetAllLanguages();
        List<SourceModel> GetAllNamedSources();
        PhraseModel AddNewPhrase(string content, string notes);
        void UpdatePhrase(UserModel loggedInUser, string? phraseId, string? content, string? notes);
    }
}