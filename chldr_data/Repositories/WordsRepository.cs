using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Realms.Sync;

namespace chldr_data.Repositories
{
    public class WordsRepository : Repository<SqlWord, WordModel, WordDto>
    {
        protected override RecordType RecordType => RecordType.Word;
        public WordsRepository(SqlContext context) : base(context) { }

        public override IEnumerable<ChangeSetModel> Add(string userId, WordDto dto)
        {
            var entity = SqlWord.FromDto(dto);
            SqlContext.Add(entity);

            var wordChangeSetDto = new ChangeSetDto()
            {
                Operation = Operation.Insert,
                UserId = userId!,
                RecordId = dto.WordId,
                RecordType = RecordType,
            };
            using var unitOfWork = new UnitOfWork(SqlContext);
            unitOfWork.ChangeSets.Add(userId, wordChangeSetDto);

            // Return changeset with updated index
            var entryChangeSetModel = unitOfWork.ChangeSets.Get(wordChangeSetDto.ChangeSetId);
            return new List<ChangeSetModel>() { entryChangeSetModel };
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

        public override IEnumerable<ChangeSetModel> Update(string userId, WordDto updatedWordDto)
        {
            var existingWordEntity = SqlContext.Words.Find(updatedWordDto.WordId);
            if (existingWordEntity == null)
            {
                throw new NullReferenceException();
            }

            var response = new List<ChangeSetModel>();
            var existingWordDto = WordDto.FromModel(WordModel.FromEntity(existingWordEntity));

            // Apply changes to the entry entity
            var entryChanges = UnitOfWork.GetChanges<EntryDto>(updatedWordDto, existingWordDto);
            if (entryChanges.Count != 0)
            {
                ApplyChanges(updatedWordDto.EntryId, entryChanges);

                var entryChangeSetDto = new ChangeSetDto()
                {
                    Operation = Operation.Update,
                    UserId = userId!,
                    RecordId = updatedWordDto.EntryId,
                    RecordType = RecordType.Entry,
                    RecordChanges = JsonConvert.SerializeObject(entryChanges)
                };
                using var unitOfWork = new UnitOfWork(SqlContext);
                unitOfWork.ChangeSets.Add(userId, entryChangeSetDto);

                // Return changeset with updated index
                var entryChangeSetModel = unitOfWork.ChangeSets.Get(entryChangeSetDto.ChangeSetId);
                response.Add(entryChangeSetModel);
            }

            // Apply changes to the word entity
            var wordChanges = UnitOfWork.GetChanges(updatedWordDto, existingWordDto);
            if (wordChanges.Count != 0)
            {
                ApplyChanges(updatedWordDto.WordId, wordChanges);

                var wordChangeSetDto = new ChangeSetDto()
                {
                    Operation = Operation.Update,
                    UserId = userId!,
                    RecordId = updatedWordDto.WordId,
                    RecordType = RecordType,
                    RecordChanges = JsonConvert.SerializeObject(wordChanges)
                };
                using var unitOfWork = new UnitOfWork(SqlContext);
                unitOfWork.ChangeSets.Add(userId, wordChangeSetDto);

                // Return changeset with updated index
                var wordChangeSetModel = unitOfWork.ChangeSets.Get(wordChangeSetDto.ChangeSetId);
                response.Add(wordChangeSetModel);
            }

            return response;
        }
    }
}