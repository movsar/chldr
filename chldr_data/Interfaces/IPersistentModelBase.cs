using MongoDB.Bson;

namespace chldr_data.Interfaces
{
    public interface IPersistentModelBase
    {
        public string Id { get; }
    }
}
