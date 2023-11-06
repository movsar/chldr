using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_utils;
using chldr_utils.Services;
using chldr_data.Services;

namespace chldr_data.Repositories
{
    public class ApiEntriesRepository : ApiRepository<EntryModel, EntryDto>, IEntriesRepository
    {
        #region Get and Take
        public async Task<List<EntryModel>> GetRandomsAsync(int limit)
        {
            throw new NotImplementedException();

        }
        public async Task<List<EntryModel>> GetLatestEntriesAsync()
        {
            throw new NotImplementedException();

        }
        public async Task<List<EntryModel>> GetEntriesOnModerationAsync()
        {
            throw new NotImplementedException();

        }

        public async Task<List<EntryModel>> FindAsync(string inputText, FiltrationFlags? filtrationFlags = null)
        {
            throw new NotImplementedException();

        }
        public async Task<List<EntryModel>> TakeAsync(int offset, int limit, FiltrationFlags filtrationFlags)
        {
            //var entries = await ApplyFiltrationFlags(_dbContext.All<RealmEntry>(), filtrationFlags);

            //var groupedEntries = await entries
            //    .OrderBy(e => e.RawContents)
            //    .Skip(offset)
            //    .Take(limit)
            //    .ToListAsync();

            //return groupedEntries.Select(FromEntityWithSubEntries).ToList();

            throw new NotImplementedException();
        }

        #endregion

        #region Update
        public override async Task Add(EntryDto newEntryDto)
        {
            throw new NotImplementedException();
        }
        public override async Task Update(EntryDto updatedEntryDto)
        {
            throw new NotImplementedException();

        }
        public override async Task Remove(string entityId)
        {
            throw new NotImplementedException();

        }

        public Task<ChangeSetModel> Promote(IEntry entry)
        {
            throw new NotImplementedException();
        }
        #endregion

        public async Task<int> CountAsync(FiltrationFlags filtrationFlags)
        {
            //var entries = await IEntriesRepository.ApplyFiltrationFlags(_dbContext.All<RealmEntry>(), filtrationFlags);
            //return await entries.CountAsync();

            throw new NotImplementedException();
        }


        public Task<List<EntryModel>> GetByIdsAsync(List<string> entryIds, FiltrationFlags? filtrationFlags = null)
        {
            throw new NotImplementedException();
        }

        public EntryModel FromEntry(string entryId)
        {
            throw new NotImplementedException();
        }

        protected override RecordType RecordType => RecordType.Entry;

        public ApiEntriesRepository(
            ExceptionHandler exceptionHandler,
            FileService fileService,
            RequestService requestService
            ) : base(exceptionHandler, fileService, requestService) { }


    }
}