using core.DatabaseObjects.Interfaces;

namespace core.DatabaseObjects.Models
{
    public class TranslationModel : ITranslation
    {
        public TranslationModel() { }
        public string TranslationId { get; set; }
        public string EntryId { get; set; }
        public string Content { get; set; }
        public int Rate { get; set; } = 1;
        public string UserId { get; set; }
        public string LanguageCode { get; set; }
        public string? Notes { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public static TranslationModel FromEntity(ITranslationEntity entity)
        {
            return new TranslationModel()
            {
                TranslationId = entity.TranslationId,
                EntryId = entity.EntryId,
                Content = entity.Content,
                UserId = entity.UserId,
                Notes = entity.Notes,
                Rate = entity.Rate,
                LanguageCode = entity.LanguageCode,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt                
            };
        }
    }
}
