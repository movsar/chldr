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
            return WordModel.FromEntity(
                                    word.Entry.Word,
                                    word.Entry,
                                    word.Entry.Source,
                                    word.Entry.Translations
                                        .Select(t => new KeyValuePair<ILanguageEntity, ITranslationEntity>(t.Language, t)));
        }
        public override WordModel Get(string entityId)
        {
            var word = _dbContext.Words
                          .Include(w => w.Entry)
                          .Include(w => w.Entry.User)
                          .Include(w => w.Entry.Source)
                          .Include(w => w.Entry.Translations)
                          .ThenInclude(t => t.Language)
                          .FirstOrDefault(w => w.WordId.Equals(entityId));

            if (word == null)
            {
                throw new ArgumentException($"Entity not found: {entityId}");
            }

            return FromEntity(word);
        }

        public override void Delete(string entityId)
        {
            throw new NotImplementedException();
        }

        public override void Insert(WordDto newEntryDto)
        {
            if (!newEntryDto.Translations.Any())
            {
                throw new Exception("Empty translations");
            }

            // Insert Entry entity
            var entry = SqlEntry.FromDto(newEntryDto);
            _dbContext.Add(entry);
            _dbContext.SaveChanges();

            // Set CreatedAt to update it on local entry
            newEntryDto.CreatedAt = entry.CreatedAt;

            // Insert a change set
            InsertChangeSet(Operation.Insert, _userId, newEntryDto.EntryId);
        }

        public override void Update(WordDto updatedEntryDto)
        {
            var existingEntry = Get(updatedEntryDto.EntryId);
            var existingEntryDto = EntryDto.FromModel(existingEntry);

            var updatedEntryEntity = SqlEntry.FromDto(updatedEntryDto);
            _dbContext.Update(updatedEntryEntity);
            _dbContext.SaveChanges();

            InsertEntryUpdateChangeSets(existingEntryDto, updatedEntryDto);
        }
    }
}