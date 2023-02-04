using MongoDB.Bson;

namespace chldr_data.Interfaces
{
    public interface IModelBase
    {
        public ObjectId Id { get; }
    }
}
