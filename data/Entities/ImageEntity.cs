using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace Data.Entities
{
    [MapTo("Image")]
    public class ImageEntity : RealmObject
    {
        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        public UserEntity User { get; set; }
        public WordEntity Word { get; set; }
        public string Path { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
