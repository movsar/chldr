using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    [MapTo("Language")]
    public class LanguageEntity : RealmObject
    {
        [PrimaryKey]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public string Code { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; } = DateTimeOffset.Now;
    }
}