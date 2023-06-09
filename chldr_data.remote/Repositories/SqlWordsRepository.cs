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
    public class SqlWordsRepository : SqlRepository<SqlWord, WordModel, WordDto>, IWordsRepository
    {
        public SqlWordsRepository(SqlContext context) : base(context) { }
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
        public override async Task Insert(string userId, WordDto dto)
        {
            var entity = SqlWord.FromDto(dto);
            _dbContext.Add(entity);

            InsertChangeSet(Operation.Insert, userId, dto.WordId);
        }
        private async Task ApplyEntryTranslationChanges(EntryDto existingEntryDto, EntryDto updatedEntryDto, SqlTranslationsRepository translationsRepository)
        {
            var existingTranslationIds = existingEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedTranslationIds = updatedEntryDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var insertedTranslations = updatedEntryDto.Translations.Where(t => !existingTranslationIds.Contains(t.TranslationId));
            var deletedTranslations = existingEntryDto.Translations.Where(t => !updatedTranslationIds.Contains(t.TranslationId));
            var updatedTranslations = updatedEntryDto.Translations.Where(t => existingTranslationIds.Contains(t.TranslationId) && updatedTranslationIds.Contains(t.TranslationId));

            foreach (var translationDto in insertedTranslations)
            {
                await translationsRepository.Insert(translationDto);
            }

            foreach (var translationDto in deletedTranslations)
            {
                await translationsRepository.Delete(translationDto.TranslationId);
            }

            foreach (var translationDto in updatedTranslations)
            {
                await translationsRepository.Update(translationDto);
            }
        }
        public async Task Update(string userId, WordDto updatedWordDto, ITranslationsRepository translationsRepository)
        {
            var existingWordEntity = Get(updatedWordDto.WordId);
            if (existingWordEntity == null)
            {
                throw new NullReferenceException();
            }

            var existingWordDto = WordDto.FromModel(existingWordEntity);

            // Update translations
            await ApplyEntryTranslationChanges(existingWordDto, updatedWordDto, translationsRepository as SqlTranslationsRepository);

            // Apply changes to the word entity
            var wordChanges = Change.GetChanges(updatedWordDto, existingWordDto);
            if (wordChanges.Count != 0)
            {
                ApplyChanges<SqlWord>(updatedWordDto.WordId, wordChanges);
                InsertChangeSet(Operation.Update, userId, updatedWordDto.WordId, wordChanges);
            }

            // Apply changes to the entry entity
            var entryChanges = Change.GetChanges<EntryDto>(updatedWordDto, existingWordDto);
            if (entryChanges.Count != 0)
            {
                ApplyChanges<SqlEntry>(updatedWordDto.EntryId, entryChanges);
                InsertChangeSet(Operation.Update, userId, updatedWordDto.EntryId, entryChanges);
            }
        }

        public EntryModel GetByEntryId(string entryId)
        {
            throw new NotImplementedException();
        }

        public override Task Update(string userId, WordDto dto)
        {
            throw new NotImplementedException();
        }
    }
}