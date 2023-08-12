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

        public static List<IEntryEntity> GroupWithSubentries(IQueryable<IEntryEntity> sourceEntries, IQueryable<IEntryEntity> filteredEntries)
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

            return resultingEntries;
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

        public static IQueryable<TEntity> ApplyFiltrationFlags<TEntity>(IQueryable<TEntity> sourceEntries, FiltrationFlags filtrationFlags)
         where TEntity : IEntryEntity
        {
            // Must not initiate any asynchronous operations!

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
