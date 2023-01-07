using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace Data.Entities
{
    [MapTo("Phrase")]
    public class PhraseEntity: RealmObject
    {
        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        public EntryEntity Entry { get; set; }
        [Indexed]
        public string Content { get; set; }
        [Indexed]
        public string Notes { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
        private string GetCombinedPhraseContents(PhraseEntity phrase)
        {
            var data = new List<string>();
            if (!String.IsNullOrWhiteSpace(phrase.Content))
                data.Add(phrase.Content);
            if (!String.IsNullOrWhiteSpace(phrase.Notes))
                data.Add(phrase.Notes);

            var res = String.Join(";", data);
            return res.ToLower();
        }
    }
}
