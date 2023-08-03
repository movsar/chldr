using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Models;

namespace chldr_data.Interfaces.Repositories
{
    public interface IEntriesRepository : IRepository<EntryModel, EntryDto>
    {
        protected static IQueryable<TEntity> GetFilteredEntries<TEntity>(IQueryable<TEntity> entries, FiltrationFlags filtrationFlags)
            where TEntity : IEntryEntity
        {
            if (!string.IsNullOrWhiteSpace(filtrationFlags.StartsWith))
            {
                var str = filtrationFlags.StartsWith.ToLower();
                entries = entries.Where(e => e.RawContents.StartsWith(str));
            }

            if (filtrationFlags.GroupWithSubEntries)
            {
                entries = entries.Where(e => e.ParentEntryId == null);
            }

            if (filtrationFlags.OnModeration)
            {
                entries = entries.Where(e => e.Rate > UserModel.MemberRateRange.Upper);
            }

            var entryTypes = filtrationFlags.EntryTypes.Select(et => (int)et).ToArray();
            entries = entries.Where(e => entryTypes.Contains(e.Type));

            return entries;
        }

        event Action<SearchResultModel>? GotNewSearchResult;
        Task<int> CountAsync(FiltrationFlags filtrationFlags);
        Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags filtrationFlags);
        Task<List<EntryModel>> FindAsync(string inputText, FiltrationFlags filtrationFlags);
        Task FindDeferredAsync(string inputText, FiltrationFlags filtrationFlags);
        List<EntryModel> GetEntriesOnModeration();
        List<EntryModel> GetLatestEntries();
        Task<ChangeSetModel> Promote(IEntry entry);
    }
}
