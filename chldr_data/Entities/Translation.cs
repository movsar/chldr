using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;

namespace chldr_data.Entities
{
    public class Translation : RealmObject, IEntity
    {
        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId(DateTime.Now);
        public Entry Entry { get; set; }
        public User User { get; set; }
        [Indexed]
        public string Content { get; set; } = string.Empty;
        public string RawContents { get; set; }
        public string Notes { get; set; } = string.Empty;
        public Language Language { get; set; }
        public int Rate { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;

    }
}
