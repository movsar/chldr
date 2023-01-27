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
    public class LanguageModel
    {
        public ObjectId LanguageId { get; }
        public string Code { get; } = string.Empty;
        public LanguageModel(Language languageEntity)
        {
            LanguageId = languageEntity._id;
            Code = languageEntity.Code;
        }
    }
}
