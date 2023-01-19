using chldr_data.Entities;
using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Models
{
    public class PhraseModel : EntryModel
    {
        public new ObjectId EntityId { get; }
        public string Content { get; }
        public new string Notes { get; }
        public PhraseModel(Entities.Phrase phrase) : base(phrase.Entry)
        {
            EntityId = phrase._id;
            Content = phrase.Content;
            Notes = phrase.Notes;
        }
        public PhraseModel(Entities.Entry entry) : this(entry.Phrase) { }
    }
}
