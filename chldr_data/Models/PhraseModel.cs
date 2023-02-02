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
        // This is Phrase Id, the Entry Id is in its parent
        public new ObjectId Id { get; }
        public string Content { get; }
        public string? Notes { get; }
        public PhraseModel(Entry entry) : this(entry.Phrase) { }
        public PhraseModel(Phrase phrase) : base(phrase.Entry)
        {
            Id = phrase._id;
            Content = phrase.Content;
            Notes = phrase.Notes;
        }
    }
}
