using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace chldr_data.Entities
{
    public class Activity : RealmObject, IEntity
    {
        [PrimaryKey]
        public ObjectId _id { get; set; }
        public User User { get; set; }
        public ObjectId ObjectId { get; set; }
        public string ObjectClass { get; set; }
        public string ObjectProperty { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Notes { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
