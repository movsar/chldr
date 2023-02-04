using chldr_data.Interfaces;
using MongoDB.Bson;

namespace chldr_data.Models
{
    public abstract class ModelBase : IModelBase
    {
        public ObjectId Id { get; }
        public DateTimeOffset CreatedAt { get; }
        public DateTimeOffset UpdatedAt { get; }
        public ModelBase(IEntity entity)
        {
            Id = entity._id;
            CreatedAt = entity.CreatedAt;
            UpdatedAt = entity.UpdatedAt;
        }
    }
}
