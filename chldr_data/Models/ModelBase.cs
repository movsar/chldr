using chldr_data.Interfaces;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
