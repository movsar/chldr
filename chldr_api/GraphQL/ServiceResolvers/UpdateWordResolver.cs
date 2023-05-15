using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.ResponseTypes;
using chldr_data.SqlEntities;
using chldr_tools;

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
            var word = await dbContext.Words.FindAsync(wordId);
            if (word == null)
            {
                throw new ArgumentException("Word not found", nameof(wordId));
            }

            // Update the word fields
            word.Content = content;
            word.PartOfSpeech = partOfSpeech;
            word.Notes = notes;

            // Update or create translations
            foreach (var translationDto in translationDtos)
            {
                var translation = await dbContext.Translations.FindAsync(translationDto.TranslationId);

                if (translation == null)
                {
                    if (!string.IsNullOrEmpty(translationDto.TranslationId))
                    {
                        throw new Exception("Translation Id is not empty while translation is null");
                    }
                    dbContext.Translations.Add(new SqlTranslation(translationDto));
                }

                translation.Content = translationDto.Content;
                translation.LanguageId = dbContext.Languages.Single(l => l.Code.Equals(translationDto.LanguageCode)).LanguageId;
                translation.Notes = translationDto.Notes;
            }

            var changeset = new SqlChangeSet()
            {
                Operation = chldr_data.Enums.Operation.Update,
                RecordId = word.EntryId,
                RecordType = chldr_data.Enums.RecordType.entry,
                UserId = userId
            };

            dbContext.Add(changeset);

            await dbContext.SaveChangesAsync();

            var entry = dbContext.Entries.Single(e => e.EntryId.Equals(word.EntryId));
            changeset = dbContext.ChangeSets.Single(c => c.ChangeSetId.Equals(changeset.ChangeSetId));

            return new UpdateResponse() { Success = true, Entry = entry };
        }
    }
}
