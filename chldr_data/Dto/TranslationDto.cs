using chldr_data.Models;
using MongoDB.Bson;

namespace chldr_data.Dto
{
    public class TranslationDto
    {
        public string? TranslationId { get; }
        public string? EntryId { get; }
        public string Content { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string LanguageCode { get; set; }
        public int Rate { get; set; } = 1;
        public TranslationDto(string languageCode)
        {
            LanguageCode = languageCode;
        }
        public TranslationDto(TranslationModel translation)
        {
            TranslationId = translation.Id.ToString();
            EntryId = translation.EntryId.ToString();
            Content = translation.Content;
            Notes = translation.Notes;
            Rate = translation.Rate;
            LanguageCode = translation.Language.Code;
        }
    }
}
