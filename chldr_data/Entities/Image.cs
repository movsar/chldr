using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
namespace chldr_data.Entities
{
    public class Image : RealmObject, IEntity
    {
        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId(DateTime.Now);
        public User User { get; set; }
        public Word Word { get; set; }
        public string Path { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
