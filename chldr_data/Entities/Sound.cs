using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
namespace chldr_data.Entities
{
    public class Sound : RealmObject, IEntity
    {
        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId(DateTime.Now);
        // Has to be updated
        public int UserId { get; set; }
        // Has to be updated
        public int WordId { get; set; }
        // Has to be updated
        public string Path { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
