﻿using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace chldr_data.Entities
{
    public class Image : RealmObject, IEntity
    {
        [PrimaryKey]
        public ObjectId _id { get; set; }
        public User User { get; set; }
        public Word Word { get; set; }
        public string Path { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
