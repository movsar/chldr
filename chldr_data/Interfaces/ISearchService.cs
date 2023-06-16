using chldr_data.DatabaseObjects.Models;
using chldr_data.Models;
using chldr_utils.Models;

namespace chldr_data.Interfaces
{
    public interface ISearchService
    {
        event Action<SearchResultModel>? GotNewSearchResult;
        Task FindAsync(string inputText, FiltrationFlags filtrationFlags);
        List<EntryModel> Find(string inputText, int limit);
        List<EntryModel> GetEntriesOnModeration();
        List<EntryModel> GetRandomEntries(int limit = 100);
        List<EntryModel> GetLatestEntries();
        List<EntryModel> GetWordsToFiddleWith();
    }
}
