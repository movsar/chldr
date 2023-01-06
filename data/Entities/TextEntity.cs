﻿using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace Data.Entities
{
    [MapTo("Text")]
    public class TextEntity  : RealmObject
    {
        [Key]
        [PrimaryKey] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public EntryEntity Entry { get; set; }
        [Indexed]
        public string Content { get; set; }
        public DateTimeOffset CreatedAt { get; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
