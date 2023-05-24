using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class TranslationDto : ITranslation
    {
        public string UserId { get; set; }
        public string EntryId { get; set; }
        public string LanguageCode { get; set; }
        public string Content { get; set; }
        public int Rate { get; set; } = 1;
        public string TranslationId { get; set; } = Guid.NewGuid().ToString();
        public string? LanguageId { get; set; }
        public string? Notes { get; set; }
        public TranslationDto() { }
        public TranslationDto(string entryId, string userId, LanguageModel language)
        {
            EntryId = entryId;
            UserId = userId;
            LanguageCode = language.Code;
            LanguageId = language.LanguageId;
        }
        public static TranslationDto FromModel(TranslationModel translation)
        {
            if (translation == null || translation.Language == null)
            {
                throw new Exception("Language and translation model must not be empty");
            }

            return new TranslationDto()
            {
                UserId = translation.UserId,
                TranslationId = translation.TranslationId,
                LanguageId = translation.Language.LanguageId,
                EntryId = translation.EntryId.ToString(),

                LanguageCode = translation.Language.Code,
                Content = translation.Content,
                Notes = translation.Notes,
                Rate = translation.Rate
            };
        }
    }
}
