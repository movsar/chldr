using chldr_data.Models;
using MongoDB.Bson;
using Realms.Sync;

namespace chldr_data.Interfaces
{
    public interface IDataAccess
    {
        App App { get; }

        event Action DatabaseInitialized;
        event Action<SearchResultsModel> GotResults;

        Task FindAsync(string inputText);
        PhraseModel GetPhraseById(ObjectId entityId);
        IEnumerable<EntryModel> GetRandomEntries();
        WordModel GetWordById(ObjectId entityId);
        Task InitializeDatabase();
        Task Login(string email, string password);
        Task<bool> RegisterNewUserAsync(string email, string password, string username, string firstName, string lastName);
        Task<bool> SendPasswordResetRequestAsync(string email);
        Task<bool> UpdatePasswordAsync(string token, string tokenId, string newPassword);
    }
}