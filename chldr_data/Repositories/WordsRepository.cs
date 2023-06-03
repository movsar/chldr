using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Interfaces;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using Realms.Sync;

namespace chldr_data.Repositories
{
    public class WordsRepository : Repository<SqlWord, WordModel, WordDto>, IWordsRepository
    {
        public WordsRepository(SqlContext context) : base(context) { }

        public override void Add(WordDto dto)
        {
            var entity = SqlWord.FromDto(dto);
            SqlContext.Add(entity);
        }

        public void AddRange(IEnumerable<WordDto> dtos)
        {
            var entities = dtos.Select(d => SqlWord.FromDto(d));
            SqlContext.AddRange(entities);
        }

        public override WordModel Get(string entityId)
        {
            var word = SqlContext.Words
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

            return WordModel.FromEntity(word);
        }


        public override IEnumerable<ChangeSetModel> Update(WordDto dto)
        {
            var changeSets = new List<ChangeSetDto>();

            // Update SqlWord
            var updateWordChangeSet = new ChangeSetDto()
            {
                Operation = chldr_data.Enums.Operation.Update,
                UserId = user.UserId!,
                RecordId = updatedWordDto.WordId,
                RecordType = chldr_data.Enums.RecordType.Word,
            };

            var wordChanges = unitOfWork.GetChanges(updatedWordDto, existingWordDto);
            if (wordChanges.Count != 0)
            {
                unitOfWork.ApplyChanges<SqlWord>(updatedWordDto.WordId, wordChanges);

                updateWordChangeSet.RecordChanges = JsonConvert.SerializeObject(wordChanges);
                changeSets.Add(updateWordChangeSet);
            }

            var updateEntryChangeSet = new ChangeSetDto()
            {
                Operation = chldr_data.Enums.Operation.Update,
                UserId = user.UserId!,
                RecordId = updatedWordDto.EntryId,
                RecordType = chldr_data.Enums.RecordType.Entry,
            };

            var entryChanges = unitOfWork.GetChanges<EntryDto>(updatedWordDto, existingWordDto);
            if (entryChanges.Count != 0)
            {
                unitOfWork.ApplyChanges<SqlEntry>(updatedWordDto.EntryId, entryChanges);

                updateEntryChangeSet.RecordChanges = JsonConvert.SerializeObject(entryChanges);
                changeSets.Add(updateEntryChangeSet);
            }

        }
    }
}