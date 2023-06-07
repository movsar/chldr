using chldr_utils.Models;

namespace chldr_data.Interfaces
{
    public interface ISearchEngine
    {
        Task FindAsync(string inputText, FiltrationFlags filtrationFlags);

    }
}
