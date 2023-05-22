using chldr_data.Entities;
using chldr_data.Interfaces.DatabaseEntities;

namespace chldr_data.Models
{
    public class LanguageModel : ILanguage
    {
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
