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
        public static async Task<IQueryable<TEntity>> ApplyFiltrationFlags<TEntity>(IQueryable<TEntity> sourceEntries, FiltrationFlags filtrationFlags)
         where TEntity : IEntryEntity
        {
            // Leave only selected entry types
            var entryTypes = filtrationFlags.EntryTypes.Select(et => (int)et).ToArray();
            var resultingEntries = sourceEntries.Where(e => entryTypes.Contains(e.Type));

            // If StartsWith specified, remove all that don't start with that string
            if (!string.IsNullOrWhiteSpace(filtrationFlags.StartsWith))
            {
                var str = filtrationFlags.StartsWith.ToLower();
                resultingEntries = resultingEntries.Where(e => e.RawContents.StartsWith(str));
            }

            // Don't include entries on moderation, if not specified otherwise
            if (!filtrationFlags.IncludeOnModeration)
            {
                resultingEntries = resultingEntries.Where(e => e.Rate > UserModel.MemberRateRange.Upper);
            }

            return resultingEntries;
        }

        Task<int> CountAsync(FiltrationFlags filtrationFlags);
        Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags filtrationFlags);
        Task<List<EntryModel>> FindAsync(string inputText);
        Task<List<EntryModel>> GetEntriesOnModerationAsync();
        Task<List<EntryModel>> GetLatestEntriesAsync();
        Task<ChangeSetModel> Promote(IEntry entry);
    }
}
