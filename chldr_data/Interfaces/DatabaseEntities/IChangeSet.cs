using chldr_data.Enums;

namespace chldr_data.Interfaces.DatabaseEntities
{
    internal interface IChangeSet : IEntity
    {
        public int ChangeSetId { get; set; }
        public long SequenceNumber { get; set; }
        public string TableName { get; set; }
        public int EntityId { get; set; }
        public Operation Operation { get; set; }
    }
}
