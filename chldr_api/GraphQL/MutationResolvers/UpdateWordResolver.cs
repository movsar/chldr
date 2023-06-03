using chldr_data.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.ResponseTypes;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using chldr_data.Repositories;

namespace chldr_api.GraphQL.ServiceResolvers
{
    public class UpdateWordResolver
    {
        internal async Task<UpdateResponse> ExecuteAsync(UnitOfWork unitOfWork, UserDto userDto, WordDto updatedWordDto)
        {
            //var user = UserModel.FromDto(userDto);
            var existingWord = unitOfWork.Words.Get(updatedWordDto.WordId);

            // Create a dto based on the existing object
            var existingWordDto = WordDto.FromModel(existingWord);

            var translationChangeSets = UpdateTranslations(unitOfWork, userDto, existingWordDto, updatedWordDto);
            var wordChangeSets = UpdateWordEntry(unitOfWork, userDto, existingWordDto, updatedWordDto);

            // Apply changes
            var changesets = translationChangeSets.Union(wordChangeSets);
            unitOfWork.ChangeSets.AddRange(changesets);
            await unitOfWork.SaveChangesAsync();      

            var changeSetIds = changesets.Select(c => c.ChangeSetId);
            var allChangeSets = unitOfWork.ChangeSets.ToList();

            // Retrieve back the same changesets so that they'll have indexes
            var updatedChangeSets = unitOfWork.ChangeSets.Where(c => changeSetIds.Contains(c.ChangeSetId));

            var response = new UpdateResponse() { Success = true };
            response.ChangeSets.AddRange(updatedChangeSets.Select(c => ChangeSetDto.FromModel(ChangeSetModel.FromEntity(c))));
            return response;
        }

        private List<ChangeSetDto> UpdateWordEntry(UnitOfWork unitOfWork, UserDto user, WordDto existingWordDto, WordDto updatedWordDto)
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

            return changeSets;
        }
        private List<ChangeSetDto> UpdateTranslations(UnitOfWork unitOfWork, UserDto user, WordDto existingWordDto, WordDto updatedWordDto)
        {
            var changeSets = new List<ChangeSetDto>();
            // Create a changeset with all the differences between existing and updated objects
            // ! There should be a separate ChangeSet for each changed / inserted / deleted object
            var existingTranslationIds = existingWordDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedTranslationIds = updatedWordDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var insertedTranslations = updatedWordDto.Translations.Where(t => !existingTranslationIds.Contains(t.TranslationId));
            var deletedTranslations = existingWordDto.Translations.Where(t => !updatedTranslationIds.Contains(t.TranslationId));
            var updatedTranslations = updatedWordDto.Translations.Where(t => existingTranslationIds.Contains(t.TranslationId) && updatedTranslationIds.Contains(t.TranslationId));

            foreach (var insertedTranslation in insertedTranslations)
            {
                unitOfWork.Translations.Add(insertedTranslation);

                changeSets.Add(new ChangeSetDto()
                {
                    Operation = chldr_data.Enums.Operation.Insert,
                    UserId = user.UserId!,
                    RecordId = insertedTranslation.TranslationId!,
                    RecordType = chldr_data.Enums.RecordType.Translation,
                });
            }

            foreach (var deletedTranslation in deletedTranslations)
            {
                var translation = unitOfWork.Translations.Get(deletedTranslation.TranslationId);
                if (translation == null)
                {
                    throw new NullReferenceException();
                }

                unitOfWork.Translations.Delete(translation.TranslationId);

                changeSets.Add(new ChangeSetDto()
                {
                    Operation = chldr_data.Enums.Operation.Delete,
                    UserId = user.UserId!,
                    RecordId = deletedTranslation.TranslationId!,
                    RecordType = chldr_data.Enums.RecordType.Translation,
                });
            }

            foreach (var updatedTranslation in updatedTranslations)
            {
                var sqlTranslation = unitOfWork.Translations.Get(updatedTranslation.TranslationId);
                if (sqlTranslation == null)
                {
                    throw new NullReferenceException();
                }

                var updateTranslationChangeSet = new ChangeSetDto()
                {
                    Operation = chldr_data.Enums.Operation.Update,
                    UserId = user.UserId!,
                    RecordId = updatedTranslation.TranslationId!,
                    RecordType = chldr_data.Enums.RecordType.Translation,
                };

                var changes = unitOfWork.GetChanges(updatedTranslation, existingWordDto.Translations.First(t => t.TranslationId!.Equals(updatedTranslation.TranslationId)));
                if (changes.Count != 0)
                {
                    unitOfWork.ApplyChanges<SqlTranslation>(sqlTranslation.TranslationId, changes);

                    updateTranslationChangeSet.RecordChanges = JsonConvert.SerializeObject(changes);
                    changeSets.Add(updateTranslationChangeSet);
                }
            }

            return changeSets;
        }
    }
}