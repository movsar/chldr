﻿using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Entities
{
    public class Language : RealmObject, IEntity
    {
        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId(DateTime.Now);
        public string? Name { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}