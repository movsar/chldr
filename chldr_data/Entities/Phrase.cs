using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
namespace chldr_data.Entities
{
    public class Phrase : RealmObject, IEntity
    {
        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId(DateTime.Now);
        public Entry Entry { get; set; }
        [Indexed]
        public string Content { get; set; }
        [Indexed]
        public string Notes { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
        internal string GetRawContents()
        {
            return Content.ToLower();
        }
    }
}
