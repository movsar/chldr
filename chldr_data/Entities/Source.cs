using chldr_data.Interfaces;
using MongoDB.Bson;
using Realms;
namespace chldr_data.Entities
{
    public class Source : RealmObject, IEntity
    {
        [PrimaryKey]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId(DateTime.Now);
        public string Name { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
    }
}
