using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_data.Services;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using Realms.Sync;

namespace chldr_data.remote.Repositories
{
    internal class SqlWordsRepository : SqlEntriesRepository<WordModel, WordDto>, IWordsRepository
    {
        public SqlWordsRepository(SqlContext context, string _userId) : base(context, _userId) { }
        public EntryModel GetByEntryId(string entryId)
        {
            throw new NotImplementedException();
        }


        protected override RecordType RecordType => RecordType.Word;
        public static WordModel FromEntity(SqlWord word)
        {
            return WordModel.FromEntity(word.Entry,
                                    word.Entry.Word,
                                    word.Entry.Source,
                                    word.Entry.Translations
                                        .Select(t => new KeyValuePair<ILanguageEntity, ITranslationEntity>(t.Language, t)));
        }
        public override WordModel Get(string entityId)
        {
            var word = _dbContext.Words
                          .Include(w => w.Entry)
                          .Include(w => w.Entry.Source)
                          .Include(w => w.Entry.User)
                          .Include(w => w.Entry.Translations)
                          .ThenInclude(t => t.Language)
                          .FirstOrDefault(w => w.WordId.Equals(entityId));

            if (word == null)
            {
                throw new ArgumentException($"Entity not found: {entityId}");
            }

            return FromEntity(word);
        }

        private void ApplyEntryTranslationChanges(EntryDto existingEntryDto, EntryDto updatedEntryDto, SqlTranslationsRepository translationsRepository)
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

        public override void Update(WordDto updatedWordDto)
        {
            var existingWordDto = WordDto.FromModel(Get(updatedWordDto.WordId));

            // Apply changes to the word entity
            var wordChanges = Change.GetChanges(updatedWordDto, existingWordDto);
            if (wordChanges.Count != 0)
            {
                ApplyChanges<SqlWord>(updatedWordDto.WordId, wordChanges);
                InsertChangeSet(Operation.Update, _userId, updatedWordDto.WordId, wordChanges);
            }

            // Apply changes to the entry entity
            var entryChanges = Change.GetChanges<EntryDto>(updatedWordDto, existingWordDto);
            if (entryChanges.Count != 0)
            {
                ApplyChanges<SqlEntry>(updatedWordDto.EntryId, entryChanges);
                InsertChangeSet(Operation.Update, _userId, updatedWordDto.EntryId, entryChanges);
            }
        }
        public void Update(EntryDto updatedEntryDto, ITranslationsRepository translationsRepository)
        {
            var updatedWordDto = (WordDto)updatedEntryDto;
            var existingWordDto = EntryDto.FromModel(Get(updatedWordDto.WordId));

            // Update translations
            ApplyEntryTranslationChanges(existingWordDto, updatedWordDto, (SqlTranslationsRepository)translationsRepository);

            // Update word
            Update((WordDto)existingWordDto);
        }
        public override void Delete(string entityId)
        {
            throw new NotImplementedException();
        }

        public override void Insert(WordDto newEntryDto)
        {
            if (newEntryDto.Translations.Any())
            {
                throw new Exception("Empty translations");
            }

            // Insert Entry entity
            var entry = SqlEntry.FromDto(newEntryDto);

            _dbContext.Add(entry);
            InsertChangeSet(Operation.Insert, _userId, newEntryDto.EntryId);
        }
    }
}