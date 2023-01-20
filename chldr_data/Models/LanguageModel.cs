using chldr_dataaccess.Entities;
using chldr_dataaccess.Interfaces;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_dataaccess.Models
{
    public class LanguageModel
    {
        public ObjectId EntityId { get; }
        public string Code { get; } = string.Empty;
        public LanguageModel(Language languageEntity)
        {
            EntityId = languageEntity._id;
            Code = languageEntity.Code;
        }
    }
}
