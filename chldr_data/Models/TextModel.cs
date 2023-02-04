using chldr_data.Entities;
using MongoDB.Bson;

namespace chldr_data.Models
{
    public class TextModel : EntryModel
    {
        public new ObjectId Id { get; }
        public override string Content { get; }
        public TextModel(Entry entry) : this(entry.Text) { }
        public TextModel(Text text) : base(text.Entry)
        {
            Id = text._id;
            Content = text.Content;
        }
    }
}
