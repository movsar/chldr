using chldr_dataaccess.Models;
using MongoDB.Bson;
using Realms.Sync;

namespace chldr_dataaccess.Interfaces
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
        Task RegisterNewUser(string email, string password, string username, string firstName, string lastName);
    }
}