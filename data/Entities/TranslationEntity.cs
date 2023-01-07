
using System;
using System.Collections.Generic;
using System.Text;
using Realms;
using MongoDB.Bson;

namespace Data.Entities
{
    [MapTo("Translation")]
    public class TranslationEntity : RealmObject
    {
        [PrimaryKey] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public ObjectId _id { get; set; }

        public EntryEntity Entry { get; set; }
        public UserEntity User { get; set; }
        [Indexed]
        public string Content { get; set; } = string.Empty;
        public string RawContents { get; set; }
        public string Notes { get; set; } = string.Empty;
        public LanguageEntity Language { get; set; }
        public int Rate { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;

    }
}
