using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_data.Models;

namespace chldr_data.Dto
{
    public class TranslationDto : ITranslation
    {
        public string UserId { get; set; }
        public string EntryId { get; set; }
        public string LanguageCode { get; set; }
        public string Content { get; set; }
        public int Rate { get; set; } = 1;

        public string? TranslationId { get; set; }
        public string? LanguageId { get; set; }
        public string? Notes { get; set; }

    

        public TranslationDto(string languageCode)
        {
            LanguageCode = languageCode;
        }
        public TranslationDto(SqlTranslation translation)
        {
            if (translation == null || translation.Language == null)
            {
                throw new Exception("Language and translation model must not be empty");
            }
            UserId = translation.UserId;
            TranslationId = translation.TranslationId;
            LanguageId = translation.Language.LanguageId;
            EntryId = translation.EntryId.ToString();

            LanguageCode = translation.Language.Code;
            Content = translation.Content;
            Notes = translation.Notes;
            Rate = translation.Rate;
        }
        public TranslationDto(TranslationModel translation)
        {
            if (translation == null || translation.Language == null)
            {
                throw new Exception("Language and translation model must not be empty");
            }
            UserId = translation.UserId;
            TranslationId = translation.TranslationId;
            LanguageId = translation.Language.LanguageId;
            EntryId = translation.EntryId.ToString();

            LanguageCode = translation.Language.Code;
            Content = translation.Content;
            Notes = translation.Notes;
            Rate = translation.Rate;

        }
    }
}
