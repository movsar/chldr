
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Models
{
    public class LanguageModel : ILanguage
    {
        private LanguageModel() { } 
        public string? LanguageId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public static LanguageModel FromEntity(ILanguageEntity languageEntity)
        {
            return new LanguageModel()
            {
                Code = languageEntity.Code,
                Name = languageEntity.Name ?? string.Empty
            };
        }
    }
}
