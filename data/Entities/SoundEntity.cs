using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace Data.Entities
{
    [MapTo("Sound")]
    public class SoundEntity : RealmObject
    {
        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        public int UserId { get; set; }
        public int WordId { get; set; }
        public string Path { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
