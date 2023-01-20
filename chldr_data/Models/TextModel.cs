using chldr_dataaccess.Entities;
using chldr_dataaccess.Interfaces;
using MongoDB.Bson;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_dataaccess.Models
{
    public class TextModel : EntryModel
    {
        public new ObjectId EntityId { get; }
        public string Content { get; }

        public TextModel(Entities.Entry entry) : base(entry)
        {
            EntityId = entry.Text._id;
            Content = entry.Text.Content;
        }
    }
}
