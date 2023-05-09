using chldr_data.Enums;

namespace chldr_data.Interfaces.DatabaseEntities
{
    internal interface IChangeSet : IEntity
    {
        public string ChangeSetId { get; set; }
        public string UserId { get; set; }
        public long SequenceNumber { get; set; }
        public RecordType RecordType { get; set; }
        public int RecordId { get; set; }
        public Operation Operation { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
