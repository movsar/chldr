using chldr_data.Interfaces;
using MongoDB.Bson;

namespace chldr_data.Models
{
    public abstract class PersistentModelBase : IPersistentModelBase
    {
        public ObjectId Id { get; }
        public DateTimeOffset CreatedAt { get; }
        public DateTimeOffset UpdatedAt { get; }
        public PersistentModelBase(IEntity entity)
        {
        }
    }
}
