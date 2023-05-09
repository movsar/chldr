using chldr_data.Interfaces.DatabaseEntities;
using MongoDB.Bson;

namespace chldr_data.Models
{
    public abstract class PersistentModelBase : IEntity
    {
        public DateTimeOffset CreatedAt { get; }
        public DateTimeOffset UpdatedAt { get; }
    }
}
