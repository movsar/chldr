using chldr_data.Entities;
using chldr_data.Enums;

namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface IChangeSet : IEntity
    {
        public long ChangeSetId { get; set; }
        public string UserId { get; set; }
        public string RecordId { get; set; }
        public string RecordChanges { get; set; }
        public RecordType RecordType { get; set; }
        public Operation Operation { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
