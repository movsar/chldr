using chldr_data.Models;
using MongoDB.Bson;
using Realms;
using Realms.Sync;

namespace chldr_data.Interfaces
{
    public interface IDataAccess
    {
        App App { get; }

        event Action DatabaseInitialized;
        event Action<SearchResultModel> GotNewSearchResult;

        WordModel GetWordById(ObjectId entityId);
        PhraseModel GetPhraseById(ObjectId entityId);
        void LoadRandomEntries();
        Task FindAsync(string inputText);
        Task LogInEmailPasswordAsync(string email, string password);
        Task RegisterNewUserAsync(string email, string password);
        Task SendPasswordResetRequestAsync(string email);
        Task UpdatePasswordAsync(string token, string tokenId, string newPassword);
        Task ConfirmUserAsync(string token, string tokenId, string email);
        Task LogOutAsync();
        Task<UserModel?> GetCurrentUserInfoAsync();
        PhraseModel GetPhraseByEntryId(ObjectId entryId);
        WordModel GetWordByEntryId(ObjectId entryId);
    }
}