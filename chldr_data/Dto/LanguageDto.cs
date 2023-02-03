using chldr_data.Entities;
using chldr_data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
