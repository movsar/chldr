using domain.DatabaseObjects.Interfaces;
using domain.DatabaseObjects.Models;
using domain.Models;

namespace domain.DatabaseObjects.Dtos
{
    public class TranslationDto : ITranslation
    {
        public string UserId { get; set; }
        public string EntryId { get; set; }
        public string SourceId { get; set; }
        public string LanguageCode { get; set; }
        public string Content { get; set; }
        public int Rate { get; set; } = 1;
        public string TranslationId { get; set; } = Guid.NewGuid().ToString();
        public string? Notes { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public TranslationDto() { }
        public TranslationDto(string entryId, string userId, LanguageModel language)
        {
            EntryId = entryId;
            UserId = userId;
            LanguageCode = language.Code;
        }
        public static TranslationDto FromModel(TranslationModel translation)
        {
            if (translation == null || string.IsNullOrEmpty(translation.LanguageCode))
            {
                throw new Exception("Language and translation model must not be empty");
            }

            return new TranslationDto()
            {
                UserId = translation.UserId,
                TranslationId = translation.TranslationId,
                EntryId = translation.EntryId.ToString(),
                SourceId = translation.SourceId.ToString(),
                LanguageCode = translation.LanguageCode,                
                Content = translation.Content,
                Notes = translation.Notes,
                Rate = translation.Rate,
                CreatedAt = translation.CreatedAt,
                UpdatedAt = translation.UpdatedAt
            };
        }
    }
}
