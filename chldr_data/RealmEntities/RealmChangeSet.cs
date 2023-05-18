using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Interfaces.DatabaseEntities;
using Realms;

namespace chldr_data.RealmEntities
{
    [MapTo("ChangeSet")]
    public class RealmChangeSet : RealmObject, IChangeSet, IEntity
    {
        private int _operation;
        private int _recordType;

        [PrimaryKey]
        public long ChangeSetId { get; set; }
        public string UserId { get; set; }
        public string RecordId { get; set; }
        public string RecordChanges { get; set; }

        public RecordType RecordType
        {
            get => (RecordType)_recordType;
            set => _recordType = (int)value;
        }
        public Operation Operation
        {
            get => (Operation)_operation;
            set => _operation = (int)value;
        }
        public DateTimeOffset CreatedAt { get; set; }
        public RealmChangeSet() { }
        public RealmChangeSet(IChangeSet changeSet)
        {
            ChangeSetId = changeSet.ChangeSetId;
            UserId = changeSet.UserId;
            RecordId = changeSet.RecordId;
            RecordType = changeSet.RecordType;
            Operation = changeSet.Operation;
            CreatedAt = changeSet.CreatedAt;
        }
    }
}
