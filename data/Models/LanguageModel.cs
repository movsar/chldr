﻿using Data.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class LanguageModel
    {
        public ObjectId EntityId { get; }
        public string Code { get; } = string.Empty;
        public LanguageModel(LanguageEntity languageEntity)
        {
            EntityId = languageEntity.Id;
            Code = languageEntity.Code;
        }
    }
}
