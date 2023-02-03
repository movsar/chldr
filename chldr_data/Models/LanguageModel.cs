using chldr_data.Entities;
using chldr_data.Interfaces;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
