using chldr_data.Dto;
using chldr_data.ResponseTypes;
using chldr_data.SqlEntities;
using chldr_tools;
using Microsoft.EntityFrameworkCore;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace chldr_api.GraphQL.ServiceResolvers
{
    public class UpdateWordResolver
    {
        public List<ChangeDto> GetChanges(WordDto updatedDto, WordDto existingDto, string changeSetId)
        {
            var changes = new List<ChangeDto>();
            var properties = typeof(WordDto).GetProperties();

            foreach (var property in properties)
            {
                var currentValue = property.GetValue(updatedDto);
                var existingValue = property.GetValue(existingDto);
                
                if (!Equals(JsonConvert.SerializeObject(currentValue), JsonConvert.SerializeObject(existingValue)))
                {
                    changes.Add(new ChangeDto()
                    {
                        Property = property.Name,
                        NewValue = currentValue,
                        OldValue = existingValue,
                        ChangeSetId = changeSetId
                    });
                }
            }

            return changes;
        }

        internal async Task<UpdateResponse> ExecuteAsync(SqlContext dbContext, UserDto userDto, WordDto updatedWordDto)
        {
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
            var changeset = new SqlChangeSet()
            {
                Operation = (int)chldr_data.Enums.Operation.Update,
                UserId = userDto.UserId,
                RecordId = existingSqlWord.EntryId,
                RecordType = (int)chldr_data.Enums.RecordType.word,
            };
            var changes = GetChanges(updatedWordDto, existingWordDto, changeset.ChangeSetId);

            // Apply changes
            dbContext.Add(changeset);
            await dbContext.SaveChangesAsync();

            // Convert to a word dto
            var wordEntryEntity = dbContext.Entries
                .Include(e => e.Source)
                .Include(e => e.User)
                .First(e => e.EntryId.Equals(existingSqlWord.EntryId));

            changeset = dbContext.ChangeSets.Single(c => c.ChangeSetId.Equals(changeset.ChangeSetId));
            string serializedObject = JsonConvert.SerializeObject(existingWordDto);


            var response = new UpdateResponse() { Success = true, ChangeSet = changeset };
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
