using Data.Entities;
using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class PhraseModel : TargetModel
    {
        public ObjectId EntityId { get; }
        public string Content { get; }
        public string Notes { get; }
        public PhraseModel(PhraseEntity phrase)
        {
            EntityId = phrase.Id;
            Content = phrase.Content;
            Notes = phrase.Notes;
        }
    }
}
