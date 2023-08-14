using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.Models;
using Newtonsoft.Json;

namespace chldr_data.DatabaseObjects.Models
{
    public class TranslationModel : ITranslation
    {
        private TranslationModel() { }
        public string TranslationId { get; internal set; }
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
