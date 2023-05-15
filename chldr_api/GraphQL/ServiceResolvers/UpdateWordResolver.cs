using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.ResponseTypes;
using chldr_tools;

namespace chldr_api.GraphQL.ServiceResolvers
{
    public class UpdateWordResolver
    {
        internal async Task<MutationResponse> ExecuteAsync(
                                                          SqlContext dbContext,
                                                          string wordId,
                                                          string content,
                                                          int partOfSpeech,
                                                          string notes)
                                                          //List<TranslationDto> translationDtos)
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
            //foreach (var translationDto in translationDtos)
            //{
            //    var translation = await dbContext.Translations.FindAsync(translationDto.TranslationId);

            //    if (translation == null && !string.IsNullOrEmpty(translationDto.TranslationId))
            //    {
            //        throw new Exception("Translation Id is not empty while translation is null");
            //    }
            //    dbContext.Translations.Add(new SqlTranslation(translationDto));

            //    translation.Content = translationDto.Content;
            //    translation.LanguageId = dbContext.Languages.Single(l => l.Code.Equals(translationDto.LanguageCode)).LanguageId;
            //    translation.Notes = translationDto.Notes;
            //}

            await dbContext.SaveChangesAsync();

            return new MutationResponse() { Success = true };
        }
    }
}
