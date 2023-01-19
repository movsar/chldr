using System.ComponentModel.DataAnnotations.Schema;
using Realms;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Realms.Schema;
using MongoDB.Bson;
using RequiredAttribute = Realms.RequiredAttribute;

namespace chldr_data.Entities
{
    public class Entry : RealmObject
    {
        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        public User User { get; set; }
        public Word Word { get; set; }
        public Phrase Phrase { get; set; }
        public Text Text { get; set; }
        public Source Source { get; set; }
        public int Rate { get; set; }
        public int Type { get; set; }
        // Used to increase search speed
        public string RawContents { get; set; }
        public IList<Translation> Translations { get; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
