using chldr_data.Interfaces.DatabaseEntities;
using chldr_data.Models;

namespace chldr_data.Dto
{
    public class LanguageDto : ILanguage
    {
        public string? LanguageId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
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
