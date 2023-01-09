﻿using System.ComponentModel.DataAnnotations.Schema;
using Realms;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Realms.Schema;
using MongoDB.Bson;
using RequiredAttribute = Realms.RequiredAttribute;

namespace Data.Entities
{
    [MapTo("Entry")]
    public class EntryEntity : RealmObject
    {
        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        public UserEntity User { get; set; }
        public WordEntity Word { get; set; }
        public PhraseEntity Phrase { get; set; }
        public TextEntity Text { get; set; }
        public SourceEntity Source { get; set; }
        public int Rate { get; set; }
        public int Type { get; set; }
        // Used to increase search speed
        public string RawContents { get; set; }
        public IList<TranslationEntity> Translations { get; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
