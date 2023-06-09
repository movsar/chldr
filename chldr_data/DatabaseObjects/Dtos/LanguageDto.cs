using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class LanguageDto : ILanguage
    {
        public string LanguageId { get; set; } = Guid.NewGuid().ToString();
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public static LanguageDto FromModel(LanguageModel model)
        {
            return new LanguageDto()
            {
                Code = model.Code,
                Name = model.Name
            };
        }
    }
}
