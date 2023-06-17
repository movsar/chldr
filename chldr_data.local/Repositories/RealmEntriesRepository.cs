using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_data.ResponseTypes;
using chldr_utils.Interfaces;
using chldr_utils;
using GraphQL;
using Realms;

namespace chldr_data.Repositories
{
    public class RealmEntriesRepository : RealmRepository<RealmEntry, EntryModel, EntryDto>, IEntriesRepository
    {
        public RealmEntriesRepository(Realm context, ExceptionHandler exceptionHandler) : base(context, exceptionHandler) { }
        protected override RecordType RecordType => RecordType.Entry;
        protected override EntryModel FromEntityShortcut(RealmEntry entry)
        {
            return EntryModel.FromEntity(
                                    entry,
                                    entry.Source,
                                    entry.Translations);
        }

        public override void Add(EntryDto newEntryDto)
        {
            _dbContext.Write(() =>
            {
                var newEntry = RealmEntry.FromDto(newEntryDto, _dbContext);
                _dbContext.Add(newEntry);
            });
        }

        public override void Update(EntryDto updatedEntryDto)
        {
            _dbContext.Write(() =>
            {
                var updatedEntry = RealmEntry.FromDto(updatedEntryDto, _dbContext);
            });
        }
    }
}