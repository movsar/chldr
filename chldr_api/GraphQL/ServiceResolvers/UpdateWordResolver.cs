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
        internal async Task<UpdateResponse> ExecuteAsync(
                                                          SqlContext dbContext,
                                                          string userId,
                                                          string wordId,
                                                          string content,
                                                          int partOfSpeech,
                                                          string notes,
                                                          List<TranslationDto> translationDtos)
        {
            var sqlWord = await dbContext.Words.FindAsync(wordId);
            if (sqlWord == null)
            {
                throw new ArgumentException("Word not found", nameof(wordId));
            }

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

            var existingWordDto = new WordDto(sqlWord);

            // Record changes for the sync mechanism
            var changeset = new SqlChangeSet()
            {
                Operation = chldr_data.Enums.Operation.Update,
                UserId = userId,
                RecordId = sqlWord.EntryId,
                RecordType = chldr_data.Enums.RecordType.word,
            };

            dbContext.Add(changeset);
            await dbContext.SaveChangesAsync();

            // Convert to a word dto
            var wordEntryEntity = dbContext.Entries
                .Include(e => e.Source)
                .Include(e => e.User)
                .First(e => e.EntryId.Equals(sqlWord.EntryId));

            changeset = dbContext.ChangeSets.Single(c => c.ChangeSetId.Equals(changeset.ChangeSetId));
            string serializedObject = JsonConvert.SerializeObject(existingWordDto);


            var response = new UpdateResponse() { Success = true, ChangeSet = changeset };
            return response;
        }
    }
}
