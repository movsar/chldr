﻿using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace chldr_data.Entities
{
    public class Text  : RealmObject
    {
        [PrimaryKey] 
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        public Entry Entry { get; set; }
        [Indexed]
        public string Content { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}