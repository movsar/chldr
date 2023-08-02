using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Models;
using chldr_utils.Models;

namespace chldr_data.Interfaces.Repositories
{
    public interface IEntriesRepository : IRepository<EntryModel, EntryDto>
    {
        event Action<SearchResultModel>? GotNewSearchResult;

        Task<int> CountAsync();
        Task<List<EntryModel>> FindAsync(string inputText, FiltrationFlags filtrationFlags);
        Task FindDeferredAsync(string inputText, FiltrationFlags filtrationFlags);
        List<EntryModel> GetEntriesOnModeration();
        List<EntryModel> GetLatestEntries();
        Task<ChangeSetModel> Promote(IEntry entry);
    }
}
