using chldr_data.Entities;
using MongoDB.Bson;
using static System.Net.Mime.MediaTypeNames;

namespace chldr_data.Models
{
    public class TextModel : EntryModel
    {
        public ObjectId TextId { get; }
        public override string Content { get; }
        public TextModel(Entry entry) : base(entry)
        {
            //var text = entry.Text;

            //TextId = text._id;
            //Content = text.Content;
        }
    }
}
