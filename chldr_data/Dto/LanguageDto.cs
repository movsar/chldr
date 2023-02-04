using chldr_data.Models;

namespace chldr_data.Dto
{
    public class LanguageDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public LanguageDto() { }
        public LanguageDto(LanguageModel language)
        {
            Code = language.Code;
            Name = language.Name;
        }
    }
}
