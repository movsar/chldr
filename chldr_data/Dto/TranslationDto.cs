using chldr_data.Interfaces;
using chldr_data.Models;
using MongoDB.Bson;

namespace chldr_data.Dto
{
    public class TranslationDto : ITranslation
    {
        public string? TranslationId { get; set; }
        public string? EntryId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string LanguageCode { get; set; }
        public int Rate { get; set; } = 1;
        public string UserId { get; set; }
        public string? LanguageId { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public TranslationDto(string languageCode)
        {
            LanguageCode = languageCode;
        }
        public TranslationDto(TranslationModel translation)
        {
            if (translation == null || translation.Language == null)
            {
                throw new Exception("Language and translation model must not be empty");
            }

            TranslationId = translation.TranslationId;
            EntryId = translation.EntryId.ToString();
            Content = translation.Content;
            Notes = translation.Notes;
            Rate = translation.Rate;
            LanguageCode = translation.Language.Code;
            LanguageId = translation.Language.LanguageId;
        }
    }
}
