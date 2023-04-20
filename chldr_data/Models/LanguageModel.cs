﻿using chldr_data.Entities;

namespace chldr_data.Models
{
    public class LanguageModel : PersistentModelBase
    {
        public string Code { get; } = string.Empty;
        public string Name { get; set; }
        public LanguageModel(SqlLanguage languageEntity) : base(languageEntity)
        {
            Code = languageEntity.Code;
            Name = languageEntity.Name ?? string.Empty;
        }
    }
}
