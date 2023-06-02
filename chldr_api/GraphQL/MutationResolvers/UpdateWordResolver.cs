using chldr_data.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.ResponseTypes;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace chldr_api.GraphQL.ServiceResolvers
{
    public class UpdateWordResolver
    {
        public List<Change> GetChanges<T>(T updated, T existing)
        {
            // This method compares the two dto's and returns the changed properties with their names and values

            var changes = new List<Change>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                // Get currenta and old values, use empty string if they're null
                var newValue = property.GetValue(updated) ?? "";
                var oldValue = property.GetValue(existing) ?? "";

                // ! Serialization allows comparision between complex objects, it might slow down the process though and worth reconsidering
                if (!Equals(JsonConvert.SerializeObject(newValue), JsonConvert.SerializeObject(oldValue)))
                {
                    changes.Add(new Change()
                    {
                        Property = property.Name,
                        OldValue = oldValue,
                        NewValue = newValue,
                    });
                }
            }

            return changes;
        }
        private void SetPropertyValue(object obj, string propertyName, object value)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value);
            }
        }

        internal async Task<UpdateResponse> ExecuteAsync(SqlContext dbContext, UserDto userDto, WordDto updatedWordDto)
        {
            var user = UserModel.FromDto(userDto);
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

            var existingWordDto = WordDto.FromModel(WordModel.FromEntity(existingSqlWord));

            var translationChangeSets = UpdateTranslations(dbContext, userDto, existingWordDto, updatedWordDto);
            var wordChangeSets = UpdateWordEntry(dbContext, userDto, existingWordDto, updatedWordDto);

            // Apply changes
            var changesets = translationChangeSets.Union(wordChangeSets);
            dbContext.AddRange(changesets);
            await dbContext.SaveChangesAsync();
            var wordEntryEntity = dbContext.Entries
                .Include(e => e.Source)
                .Include(e => e.User)
                .First(e => e.EntryId.Equals(existingSqlWord.EntryId));

            var changeSetIds = changesets.Select(c => c.ChangeSetId);
            var allChangeSets = dbContext.ChangeSets.ToList();
            var updatedChangeSets = dbContext.ChangeSets.Where(c => changeSetIds.Contains(c.ChangeSetId));

            response.ChangeSets.AddRange(updatedChangeSets.Select(c => ChangeSetDto.FromModel(ChangeSetModel.FromEntity(c))));

            return response;
        }

        private List<SqlChangeSet> UpdateWordEntry(SqlContext dbContext, UserDto user, WordDto existingWordDto, WordDto updatedWordDto)
        {
            var changeSets = new List<SqlChangeSet>();

            // Update SqlWord
            var updateWordChangeSet = new SqlChangeSet()
            {
                Operation = (int)chldr_data.Enums.Operation.Update,
                UserId = user.UserId!,
                RecordId = updatedWordDto.WordId,
                RecordType = (int)chldr_data.Enums.RecordType.Word,
            };

            var wordChanges = GetChanges(updatedWordDto, existingWordDto);
            if (wordChanges.Count != 0)
            {
                var sqlWord = dbContext.Words.Find(updatedWordDto.WordId);
                if (sqlWord == null)
                {
                    throw new NullReferenceException();
                }

                foreach (var change in wordChanges)
                {
                    SetPropertyValue(sqlWord, change.Property, change.NewValue);
                }

                updateWordChangeSet.RecordChanges = JsonConvert.SerializeObject(wordChanges);
                changeSets.Add(updateWordChangeSet);
            }

            var updateEntryChangeSet = new SqlChangeSet()
            {
                Operation = (int)chldr_data.Enums.Operation.Update,
                UserId = user.UserId!,
                RecordId = updatedWordDto.EntryId,
                RecordType = (int)chldr_data.Enums.RecordType.Entry,
            };
            var entryChanges = GetChanges<EntryDto>(updatedWordDto, existingWordDto);
            if (entryChanges.Count != 0)
            {
                var sqlEntry = dbContext.Entries.Find(updatedWordDto.EntryId);
                if (sqlEntry == null)
                {
                    throw new NullReferenceException();
                }

                foreach (var change in entryChanges)
                {
                    SetPropertyValue(sqlEntry, change.Property, change.NewValue);
                }

                updateEntryChangeSet.RecordChanges = JsonConvert.SerializeObject(entryChanges);
                changeSets.Add(updateEntryChangeSet);
            }

            return changeSets;
        }
        private List<SqlChangeSet> UpdateTranslations(SqlContext dbContext, UserDto user, WordDto existingWordDto, WordDto updatedWordDto)
        {
            var changeSets = new List<SqlChangeSet>();
            // Create a changeset with all the differences between existing and updated objects
            // ! There should be a separate ChangeSet for each changed / inserted / deleted object
            var existingTranslationIds = existingWordDto.Translations.Select(t => t.TranslationId).ToHashSet();
            var updatedTranslationIds = updatedWordDto.Translations.Select(t => t.TranslationId).ToHashSet();

            var insertedTranslations = updatedWordDto.Translations.Where(t => !existingTranslationIds.Contains(t.TranslationId));
            var deletedTranslations = existingWordDto.Translations.Where(t => !updatedTranslationIds.Contains(t.TranslationId));
            var updatedTranslations = updatedWordDto.Translations.Where(t => existingTranslationIds.Contains(t.TranslationId) && updatedTranslationIds.Contains(t.TranslationId));

            foreach (var insertedTranslation in insertedTranslations)
            {
                var sqlTranslation = SqlTranslation.FromDto(insertedTranslation);
                dbContext.Add(sqlTranslation);
                changeSets.Add(new SqlChangeSet()
                {
                    Operation = (int)chldr_data.Enums.Operation.Insert,
                    UserId = user.UserId!,
                    RecordId = insertedTranslation.TranslationId!,
                    RecordType = (int)chldr_data.Enums.RecordType.Translation,
                });
            }

            foreach (var deletedTranslation in deletedTranslations)
            {
                var sqlTranslation = dbContext.Translations.Find(deletedTranslation.TranslationId);
                if (sqlTranslation == null)
                {
                    throw new NullReferenceException();
                }

                dbContext.Remove(sqlTranslation);

                changeSets.Add(new SqlChangeSet()
                {
                    Operation = (int)chldr_data.Enums.Operation.Delete,
                    UserId = user.UserId!,
                    RecordId = deletedTranslation.TranslationId!,
                    RecordType = (int)chldr_data.Enums.RecordType.Translation,
                });
            }

            foreach (var updatedTranslation in updatedTranslations)
            {
                var sqlTranslation = dbContext.Translations.Find(updatedTranslation.TranslationId);
                if (sqlTranslation == null)
                {
                    throw new NullReferenceException();
                }

                var updateTranslationChangeSet = new SqlChangeSet()
                {
                    Operation = (int)chldr_data.Enums.Operation.Update,
                    UserId = user.UserId!,
                    RecordId = updatedTranslation.TranslationId!,
                    RecordType = (int)chldr_data.Enums.RecordType.Translation,
                };

                var changes = GetChanges(updatedTranslation, existingWordDto.Translations.First(t => t.TranslationId!.Equals(updatedTranslation.TranslationId)));
                if (changes.Count != 0)
                {
                    foreach (var change in changes)
                    {
                        SetPropertyValue(sqlTranslation, change.Property, change.NewValue);
                    }

                    updateTranslationChangeSet.RecordChanges = JsonConvert.SerializeObject(changes);
                    changeSets.Add(updateTranslationChangeSet);
                }
            }

            return changeSets;
        }
    }
}