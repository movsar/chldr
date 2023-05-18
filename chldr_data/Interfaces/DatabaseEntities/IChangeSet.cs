using chldr_data.Enums;

namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface IChangeSet : IEntity
    {
        public long ChangeSetId { get; set; }
        public string UserId { get; set; }
        public Operation Operation { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
