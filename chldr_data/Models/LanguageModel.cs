using chldr_data.Entities;

namespace chldr_data.Models
{
    public class LanguageModel : ModelBase
    {
        public string Code { get; } = string.Empty;
        public string Name { get; set; }
        public LanguageModel(Language languageEntity) : base(languageEntity)
        {
            Code = languageEntity.Code;
            Name = languageEntity.Name ?? string.Empty;
        }
    }
}
