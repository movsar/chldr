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
    public class TextModel : EntryModel
    {
        public new ObjectId Id { get; }
        public string Content { get; }
        public TextModel(Entry entry) : this(entry.Text) { }
        public TextModel(Text text) : base(text.Entry)
        {
            Id = text._id;
            Content = text.Content;
        }
    }
}
