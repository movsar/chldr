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

        public override void Insert(EntryDto dto)
        {
            throw new NotImplementedException();
        }

        public override void Delete(string entityId)
        {
            throw new NotImplementedException();
        }
        private void ApplyEntryTranslationChanges(EntryDto existingEntryDto, EntryDto updatedEntryDto, ITranslationsRepository translationsRepository)
        {
            var existingTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var insertedTranslations = updatedEntryDto.Translations.Where(t => !existingTranslationIds.Contains(t.TranslationId));
            var deletedTranslations = existingEntryDto.Translations.Where(t => !updatedTranslationIds.Contains(t.TranslationId));
            var updatedTranslations = updatedEntryDto.Translations.Where(t => existingTranslationIds.Contains(t.TranslationId) && updatedTranslationIds.Contains(t.TranslationId));

            foreach (var translationDto in insertedTranslations)
            {
                translationsRepository.Insert(translationDto);
            }

            foreach (var translationDto in deletedTranslations)
            {
                translationsRepository.Delete(translationDto.TranslationId);
            }

            foreach (var translationDto in updatedTranslations)
            {
                translationsRepository.Update(translationDto);
            }
        }

        public override void Update(EntryDto updatedEntryDto)
        {
            var existingEntryDto = EntryDto.FromModel(Get(updatedEntryDto.EntryId));

            // Apply changes to the entry entity
            var entryChanges = Change.GetChanges<EntryDto>(updatedEntryDto, existingEntryDto);
            if (entryChanges.Count != 0)
            {
                ApplyChanges<RealmEntry>(updatedEntryDto.EntryId, entryChanges);
            }
        }
    }
}