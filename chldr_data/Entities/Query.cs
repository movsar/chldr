using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
namespace chldr_data.Entities
{
    public class Query : RealmObject, IEntity
    {
        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId(DateTime.Now);
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
