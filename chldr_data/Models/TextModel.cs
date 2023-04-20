using chldr_data.Entities;
using MongoDB.Bson;
using static System.Net.Mime.MediaTypeNames;

namespace chldr_data.Models
{
    public class TextModel : EntryModel
    {
        public string TextId { get; }
        public override string Content { get; }
        public TextModel(RealmEntry entry) : base(entry)
        {
            var text = entry.Text;

            TextId = text.TextId;
            Content = text.Content;
        }
    }
}
