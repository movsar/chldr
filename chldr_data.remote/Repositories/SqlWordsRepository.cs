using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_data.Services;
using chldr_tools;
using Microsoft.EntityFrameworkCore;

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
        public async Task Update(string userId, WordDto updatedWordDto, ITranslationsRepository translationsRepository)
        {
            var existingWordEntity = Get(updatedWordDto.WordId);
            if (existingWordEntity == null)
            {
                throw new NullReferenceException();
            }

            var existingWordDto = WordDto.FromModel(existingWordEntity);

            // Apply changes to the entry entity
            var entryChanges = SqlUnitOfWork.GetChanges<EntryDto>(updatedWordDto, existingWordDto);
            if (entryChanges.Count != 0)
            {
                ApplyChanges<SqlEntry>(updatedWordDto.EntryId, entryChanges);
                InsertChangeSet(Operation.Update, userId, updatedWordDto.EntryId, entryChanges);
            }

            // Apply changes to the word entity
            var wordChanges = SqlUnitOfWork.GetChanges(updatedWordDto, existingWordDto);
            if (wordChanges.Count != 0)
            {
                ApplyChanges<SqlWord>(updatedWordDto.WordId, wordChanges);
                InsertChangeSet(Operation.Update, userId, updatedWordDto.WordId, wordChanges);
            }

            // Update translations
            var existingTranslationIds = existingWordDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedTranslationIds = updatedWordDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var insertedTranslations = updatedWordDto.Translations.Where(t => !existingTranslationIds.Contains(t.TranslationId));
            var deletedTranslations = existingWordDto.Translations.Where(t => !updatedTranslationIds.Contains(t.TranslationId));
            var updatedTranslations = updatedWordDto.Translations.Where(t => existingTranslationIds.Contains(t.TranslationId) && updatedTranslationIds.Contains(t.TranslationId));

            foreach (var translationDto in insertedTranslations)
            {
                await translationsRepository.Insert(userId, translationDto);
            }

            foreach (var translationDto in deletedTranslations)
            {
                await translationsRepository.Delete(userId, translationDto.TranslationId);
            }

            foreach (var translationDto in updatedTranslations)
            {
                await translationsRepository.Update(userId, translationDto);
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