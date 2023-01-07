using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace Data.Entities
{
    [MapTo("Query")]
    public class QueryEntity : RealmObject
    {
        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
