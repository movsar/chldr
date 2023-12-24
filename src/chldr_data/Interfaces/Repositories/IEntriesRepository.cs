using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Models;

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
                    .Where(e => e.RawContents.StartsWith(inputText, StringComparison.OrdinalIgnoreCase)).Take(20);
            }
            else
            {
                resultingQuery = sourceEntries
                    .Where(e => e.RawContents.StartsWith(inputText, StringComparison.OrdinalIgnoreCase)).Take(30);
            }

            return resultingQuery;
        }            

        Task<int> CountAsync(FiltrationFlags? filtrationFlags = null);
        Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags? filtrationFlags = null);
        Task<List<EntryModel>> FindAsync(string inputText, FiltrationFlags? filtrationFlags = null);
        Task<List<EntryModel>> GetByIdsAsync(List<string> entryIds, FiltrationFlags? filtrationFlags = null);
        EntryModel FromEntry(string entryId);
        Task<ChangeSetModel> Promote(IEntry entry);
        Task<List<EntryModel>> GetEntriesOnModerationAsync();
        Task<List<EntryModel>> GetLatestEntriesAsync(int count);
    }
}
