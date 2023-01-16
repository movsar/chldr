using Data.Entities;
using Data.Interfaces;
using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class PhraseModel : EntryModel
    {
        public new ObjectId EntityId { get; }
        public string Content { get; }
        public new string Notes { get; }
        public PhraseModel(Entities.Entry entry) : base(entry)
        {
            EntityId = entry.Phrase._id;
            Content = entry.Phrase.Content;
            Notes = entry.Phrase.Notes;
        }
    }
}
