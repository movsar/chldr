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
        public override ObjectId Id { get; }
        public string Code { get; } = string.Empty;
        public LanguageModel(Language languageEntity)
        {
            Id = languageEntity._id;
            Code = languageEntity.Code;
        }
    }
}
