using chldr_data.DatabaseObjects.Models;
using chldr_data.Models;
using chldr_utils.Models;

namespace chldr_data.Interfaces
{
    public interface ISearchService
    {
        event Action<SearchResultModel>? GotNewSearchResult;
        Task FindAsync(string inputText, FiltrationFlags filtrationFlags);
        List<EntryModel> GetEntriesOnModeration();
        List<EntryModel> GetRandomEntries();
        List<EntryModel> GetWordsToFiddleWith();
    }
}
