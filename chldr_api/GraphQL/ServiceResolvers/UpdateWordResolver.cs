using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.ResponseTypes;
using chldr_data.SqlEntities;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using Realms;
using System.Diagnostics;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace chldr_api.GraphQL.ServiceResolvers
{
    public class UpdateWordResolver
    {
        public List<ChangeDto> GetChanges<T>(T updated, T existing, string changeSetId)
        {
            // This method compares the two dto's and returns the changed properties with their names and values

            var changes = new List<ChangeDto>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                // Get currenta and old values, use empty string if they're null
                var newValue = property.GetValue(updated) ?? "";
                var oldValue = property.GetValue(existing) ?? "";

                // ! Serialization allows comparision between complex objects, it might slow down the process though and worth reconsidering
                if (!Equals(JsonConvert.SerializeObject(newValue), JsonConvert.SerializeObject(oldValue)))
                {
                    changes.Add(new ChangeDto()
                    {
                        Property = property.Name,
                        NewValue = newValue,
                        OldValue = oldValue,
                        ChangeSetId = changeSetId
                    });
                }
            }

            return changes;
        }
        public void SetPropertyValue(object obj, string propertyName, object value)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value);
            }
        }
        internal async Task<UpdateResponse> ExecuteAsync(SqlContext dbContext, UserDto userDto, WordDto updatedWordDto)
        {
            var response = new UpdateResponse() { Success = true };

            // Try retrieving corresponding object from the database with all the related objects
            var existingSqlWord = dbContext.Words
                .Include(w => w.Entry)
                .Include(w => w.Entry.Source)
                .Include(w => w.Entry.User)
                .Include(w => w.Entry.Translations)
                .ThenInclude(t => t.Language)
                .First(w => w.WordId.Equals(updatedWordDto.WordId));

            if (existingSqlWord == null)
            {
                throw new ArgumentException($"Word not found WordId: {updatedWordDto.WordId}");
            }

            // Create a dto based on the existing object
            var existingWordDto = new WordDto(existingSqlWord);

            // Create a changeset with all the differences between existing and updated objects
            // ! There should be a separate ChangeSet for each changed / inserted / deleted object
            var existingTranslationIds = existingWordDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedTranslationIds = updatedWordDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var insertedTranslations = updatedWordDto.Translations.Where(t => !existingTranslationIds.Contains(t.TranslationId));
            var deletedTranslations = existingWordDto.Translations.Where(t => !updatedTranslationIds.Contains(t.TranslationId));
            var updatedTranslations = updatedWordDto.Translations.Where(t => existingTranslationIds.Contains(t.TranslationId) && updatedTranslationIds.Contains(t.TranslationId));

            foreach (var insertedTranslation in insertedTranslations)
            {
                response.ChangeSets.Add(new SqlChangeSet()
                {
                    Operation = (int)chldr_data.Enums.Operation.Insert,
                    UserId = userDto.UserId,
                    RecordId = existingSqlWord.EntryId,
                    RecordType = (int)chldr_data.Enums.RecordType.word,
                });
            }

            foreach (var deletedTranslation in deletedTranslations)
            {
                response.ChangeSets.Add(new SqlChangeSet()
                {
                    Operation = (int)chldr_data.Enums.Operation.Delete,
                    UserId = userDto.UserId,
                    RecordId = existingSqlWord.EntryId,
                    RecordType = (int)chldr_data.Enums.RecordType.word,
                });
            }

            foreach (var updatedTranslation in updatedTranslations)
            {
                var updateTranslationChangeSet = new SqlChangeSet()
                {
                    Operation = (int)chldr_data.Enums.Operation.Update,
                    UserId = userDto.UserId,
                    RecordId = existingSqlWord.EntryId,
                    RecordType = (int)chldr_data.Enums.RecordType.word,
                };

                var changes = GetChanges(updatedTranslation, existingWordDto.Translations.First(t => t.TranslationId!.Equals(updatedTranslation.TranslationId)), updateTranslationChangeSet.ChangeSetId);
                if (changes.Count != 0)
                {
                    updateTranslationChangeSet.RecordChanges = JsonConvert.SerializeObject(changes);
                    response.ChangeSets.Add(updateTranslationChangeSet);
                }
            }

            var updateWordChangeSet = new SqlChangeSet()
            {
                Operation = (int)chldr_data.Enums.Operation.Update,
                UserId = userDto.UserId,
                RecordId = existingSqlWord.EntryId,
                RecordType = (int)chldr_data.Enums.RecordType.word,
            };

            var wordChanges = GetChanges(updatedWordDto, existingWordDto, updateWordChangeSet.ChangeSetId);
            if (wordChanges.Count != 0)
            {
                response.ChangeSets.Add(updateWordChangeSet);
            }

            // TODO:
            // Update SqlEntry
            //var entryChanges = GetChanges(updatedWordDto, existingWordDto, changeset.ChangeSetId);

            // Apply changes


            var updatedSqlWord = new SqlWord(updatedWordDto);
            dbContext.Update(updatedSqlWord);

            dbContext.AddRange(response.ChangeSets);
            await dbContext.SaveChangesAsync();

            // Convert to a word dto
            var wordEntryEntity = dbContext.Entries
                .Include(e => e.Source)
                .Include(e => e.User)
                .First(e => e.EntryId.Equals(existingSqlWord.EntryId));

            return response;

            // Update the word fields
            //word.Content = content;
            //word.PartOfSpeech = partOfSpeech;
            //word.Notes = notes;

            //// Update or create translations
            //foreach (var translationDto in translationDtos)
            //{
            //    var translation = await dbContext.Translations.FindAsync(translationDto.TranslationId);
            //    if (translation == null)
            //    {
            //        if (!string.IsNullOrEmpty(translationDto.TranslationId))
            //        {
            //            throw new Exception("Translation Id is not empty while translation is null");
            //        }
            //        translation = new SqlTranslation(translationDto);
            //        dbContext.Translations.Add(translation);
            //    }

            //    translation.Content = translationDto.Content;
            //    translation.LanguageId = dbContext.Languages.Single(l => l.Code.Equals(translationDto.LanguageCode)).LanguageId;
            //    translation.Notes = translationDto.Notes;
            //}

            // Extract the changes
        }
    }
}
