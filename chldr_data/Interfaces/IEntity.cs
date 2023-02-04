using MongoDB.Bson;

namespace chldr_data.Interfaces
{
    public interface IEntity
    {
        public ObjectId _id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
