using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Realms;
using Realms.Sync;

namespace chldr_data.Repositories
{
    public class RealmWordsRepository : RealmRepository<SqlWord, WordModel, WordDto>, IWordsRepository
    {
        protected override RecordType RecordType => RecordType.Word;
        public RealmWordsRepository(SqlContext context) : base(context) { }
        public static WordModel FromEntity(SqlWord word)
        {
            return WordModel.FromEntity(word.Entry,
                                    word.Entry.Word,
                                    word.Entry.Source,
                                    word.Entry.Translations
                                        .Select(t => new KeyValuePair<ILanguageEntity, ITranslationEntity>(t.Language, t)));
        }

        public override IEnumerable<ChangeSetModel> Add(string userId, WordDto dto)
        {
            var entity = SqlWord.FromDto(dto);
            _context.Add(entity);

            // Insert changeset
            var changeSet = new SqlChangeSet()
            {
                Operation = (int)Operation.Insert,
                UserId = userId!,
                RecordId = dto.WordId,
                RecordType = (int)RecordType,
            };

            _context.ChangeSets.Add(changeSet);
            _context.SaveChanges();

            // Return changeset with updated index
            changeSet = _context.ChangeSets.Find(changeSet.ChangeSetId);
            return new List<ChangeSetModel>() { ChangeSetModel.FromEntity(changeSet) };
        }

        public override WordModel Get(string entityId)
        {
            var word = _context.Words
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

        public override IEnumerable<ChangeSetModel> Update(string userId, WordDto updatedWordDto)
        {
            var existingWordEntity = _context.Words.Find(updatedWordDto.WordId);
            if (existingWordEntity == null)
            {
                throw new NullReferenceException();
            }

            var response = new List<ChangeSetModel>();
            var existingWordDto = WordDto.FromModel(FromEntity(existingWordEntity));

            // Apply changes to the entry entity
            var entryChanges = SqlUnitOfWork.GetChanges<EntryDto>(updatedWordDto, existingWordDto);
            if (entryChanges.Count != 0)
            {
                ApplyChanges(updatedWordDto.EntryId, entryChanges);

                var entryChangeSet = new SqlChangeSet()
                {
                    Operation = (int)Operation.Update,
                    UserId = userId!,
                    RecordId = updatedWordDto.EntryId,
                    RecordType = (int)RecordType.Entry,
                    RecordChanges = JsonConvert.SerializeObject(entryChanges)
                };
                _context.ChangeSets.Add(entryChangeSet);
                _context.SaveChanges();

                // Return changeset with updated index
                var entryChangeSetModel = _context.ChangeSets.Find(entryChangeSet.ChangeSetId);
                response.Add(ChangeSetModel.FromEntity(entryChangeSetModel));
            }

            // Apply changes to the word entity
            var wordChanges = SqlUnitOfWork.GetChanges(updatedWordDto, existingWordDto);
            if (wordChanges.Count != 0)
            {
                ApplyChanges(updatedWordDto.WordId, wordChanges);

                var wordChangeSet = new SqlChangeSet()
                {
                    Operation = (int)Operation.Update,
                    UserId = userId!,
                    RecordId = updatedWordDto.WordId,
                    RecordType = (int)RecordType,
                    RecordChanges = JsonConvert.SerializeObject(wordChanges)
                };
                _context.ChangeSets.Add(wordChangeSet);
                _context.SaveChanges();

                // Return changeset with updated index
                var entryChangeSetModel = _context.ChangeSets.Find(wordChangeSet.ChangeSetId);
                response.Add(ChangeSetModel.FromEntity(entryChangeSetModel));
            }

            return response;
        }
    }
}