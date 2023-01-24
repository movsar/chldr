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
        event Action<SearchResultsModel> GotResults;

        WordModel GetWordById(ObjectId entityId);
        PhraseModel GetPhraseById(ObjectId entityId);
        IEnumerable<EntryModel> GetRandomEntries();
        Task FindAsync(string inputText);
        Task InitializeDatabase();
        Task<UserModel> LogInEmailPasswordAsync(string email, string password);
        Task RegisterNewUserAsync(string email, string password);
        Task SendPasswordResetRequestAsync(string email);
        Task UpdatePasswordAsync(string token, string tokenId, string newPassword);
        Task ConfirmUserAsync(string token, string tokenId, string userEmail);
    }
}