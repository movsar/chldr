using chldr_data.Entities;
using chldr_data.Interfaces.DatabaseEntities;

namespace chldr_data.Models
{
    public class LanguageModel : ILanguage
    {
        public string? LanguageId { get; set; }
        public string Code { get; } = string.Empty;
        public string Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public LanguageModel(ILanguage languageEntity)
        {
            Code = languageEntity.Code;
            Name = languageEntity.Name ?? string.Empty;
        }
    }
}
