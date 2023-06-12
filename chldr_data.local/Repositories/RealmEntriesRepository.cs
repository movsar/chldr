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
        public RealmEntriesRepository(Realm context, ExceptionHandler exceptionHandler, IGraphQLRequestSender graphQLRequestSender) : base(context, exceptionHandler, graphQLRequestSender) { }

        protected override RecordType RecordType => RecordType.Entry;

        public static EntryModel FromEntity(RealmEntry word)
        {
            return EntryModel.FromEntity(
                                    word,
                                    word.Source,
                                    word.Translations
                                        .Select(t => new KeyValuePair<ILanguageEntity, ITranslationEntity>(t.Language, t)));
        }
        public EntryModel GetByEntryId(string entryId)
        {
            var entry = _dbContext.Find<RealmEntry>(entryId);
            if (entry == null)
            {
                throw new Exception("There is no such entry in the database");
            }

            return FromEntity(entry);
        }

        public List<EntryModel> GetRandomWords(int limit)
        {
            var entries = _dbContext.All<RealmEntry>().AsEnumerable().Take(limit);
            return entries.Select(w => FromEntity(w)).ToList();
        }
        public override EntryModel Get(string entityId)
        {
            var entry = _dbContext.All<RealmEntry>().FirstOrDefault(w => w.EntryId == entityId);
            if (entry == null)
            {
                throw new Exception("There is no such word in the database");
            }

            return FromEntity(entry);
        }

        public override void Add(EntryDto newEntryDto)
        {
            if (!newEntryDto.Translations.Any())
            {
                throw new Exception("Empty translations");
            }

            var insertedEntry = RealmEntry.FromDto(newEntryDto, _dbContext);
        }

        public override void Update(EntryDto updatedEntryDto)
        {
            var updatedEntry = RealmEntry.FromDto(updatedEntryDto, _dbContext);
        }
    }
}