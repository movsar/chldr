using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Models;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.Interfaces.Repositories
{
    public interface IEntriesRepository : IRepository<EntryModel, EntryDto>
    {
      

        Task<int> CountAsync(FiltrationFlags filtrationFlags);
        Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags filtrationFlags);
        Task<List<EntryModel>> FindAsync(string inputText);
        Task<List<EntryModel>> GetEntriesOnModerationAsync();
        Task<List<EntryModel>> GetLatestEntriesAsync();
        Task<ChangeSetModel> Promote(IEntry entry);
    }
}
