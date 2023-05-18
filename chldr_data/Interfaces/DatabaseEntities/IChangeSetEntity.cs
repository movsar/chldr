using chldr_data.Entities;
using chldr_data.Enums;

namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface IChangeSetEntity : IEntity
    {
        public long ChangeSetIndex { get; set; }
        public string ChangeSetId { get; set; }
        public string UserId { get; set; }
        public string RecordId { get; set; }
        public string RecordChanges { get; set; }
        public int RecordType { get; set; }
        public int Operation { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
