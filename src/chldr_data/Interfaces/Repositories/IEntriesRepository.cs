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
        public static IQueryable<IEntryEntity> GetRandomEntries(IQueryable<IEntryEntity> sourceEntries, int limit)
        {
            // Don't use AsNoTracking - it produces uncaught exceptions

            var randomizer = new Random();

            // Fetch all EntryIds from the database
            var ids = sourceEntries.Where(e => e.ParentEntryId == null).Select(e => e.EntryId).ToList();

            // Shuffle the ids and take what's needed
            var randomlySelectedIds = ids.OrderBy(x => randomizer.Next(1, Constants.EntriesApproximateCoount)).Take(limit).ToList();

            // Fetch entries with the selected EntryIds
            var filteredEntries = sourceEntries
                .Where(entry => randomlySelectedIds.Contains(entry.EntryId));

            return filteredEntries;
        }

        public static List<EntryModel> GroupWithSubentries(IQueryable<IEntryEntity> sourceEntries, IQueryable<IEntryEntity> filteredEntries, Func<string, EntryModel> fromEntity)
        {
            // Must not initiate any asynchronous operations!

            /*
             * This method looks for entries that have parents, and takes the parents instead
             * with children added as subentries.
             * 
             * Should be called only on results after all the filtration has been done.
             * 
             * @param entries = all entries from database
             * @param resultingEntries = those that have already been filtered by some criteria
            */

            var subEntryParentIds = filteredEntries.Where(e => e.ParentEntryId != null).Select(e => e.ParentEntryId).ToArray();

            // Add subEntry parents
            var parents = sourceEntries.Where(e => subEntryParentIds.Contains(e.EntryId));
            var resultingEntries = filteredEntries.Union(parents).ToList();

            // Get standalone entry ids
            var subEntryIds = resultingEntries.Where(e => e.ParentEntryId != null).Select(e => e.EntryId).ToList();

            // Remove standalone sub entries
            resultingEntries.RemoveAll(e => subEntryIds.Contains(e.EntryId));

            var models = resultingEntries.Select(e => fromEntity(e.EntryId)).ToList();
            return models;
        }

        public static IQueryable<IEntryEntity> Find(IQueryable<IEntryEntity> sourceEntries, string inputText)
        {
            // Must not initiate any asynchronous operations!

            // This is the search algorythm, it will be exactly the same for both Sql and Realm databases
            // It takes the entity interfaces and returns ids, no DB specific operations or fields

            // @Nurmagomed - this is your domain :)

            IQueryable<IEntryEntity> resultingQuery;
            if (string.IsNullOrEmpty(inputText) || inputText.Length <= 3)
            {
                resultingQuery = sourceEntries
                    .Where(e => e.RawContents.Equals(inputText, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                resultingQuery = sourceEntries
                    .Where(e => e.RawContents.StartsWith(inputText, StringComparison.OrdinalIgnoreCase));
            }

            return resultingQuery;
        }

        public static IQueryable<TEntity> ApplyEntryFilters<TEntity>(IQueryable<TEntity> sourceEntries, EntryFilters? entryFilters)
         where TEntity : IEntryEntity
        {
            if (entryFilters == null)
            {
                return sourceEntries;
            }

            // Must not initiate any asynchronous operations!

            // Leave only selected entry types
            var resultingEntries = sourceEntries;
            if (entryFilters.EntryTypes != null && entryFilters.EntryTypes.Length > 0)
            {
                var entryTypes = entryFilters.EntryTypes.Select(et => (int)et).ToArray();
                resultingEntries = sourceEntries.Where(e => entryTypes.Contains(e.Type));
            }

            // If StartsWith specified, remove all that don't start with that string
            if (!string.IsNullOrWhiteSpace(entryFilters.StartsWith))
            {
                var str = entryFilters.StartsWith.ToLower();
                resultingEntries = resultingEntries.Where(e => e.RawContents.StartsWith(str));
            }

            // Don't include entries on moderation, if not specified otherwise
            if (entryFilters.IncludeOnModeration != null && entryFilters.IncludeOnModeration == false)
            {
                resultingEntries = resultingEntries.Where(e => e.Rate > UserModel.MemberRateRange.Upper);
            }

            return resultingEntries;
        }

        EntryModel FromEntry(string entryId);
        Task<int> CountAsync(FiltrationFlags filtrationFlags);
        Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags filtrationFlags);
        Task<List<EntryModel>> FindAsync(string inputText);
        Task<List<EntryModel>> GetEntriesOnModerationAsync();
        Task<List<EntryModel>> GetLatestEntriesAsync();
        Task<ChangeSetModel> Promote(IEntry entry);
        Task<List<EntryModel>> GetByIdsAsync(List<string> entryIds, FiltrationFlags? filtrationFlags = null);
    }
}
