﻿using chldr_data.Interfaces.DatabaseEntities;
using Realms;

namespace chldr_data.RealmEntities
{
    [MapTo("ChangeSet")]
    public class RealmChangeSet : RealmObject, IChangeSetEntity, IEntity
    {
        [PrimaryKey]
        public long ChangeSetIndex { get; set; }
        public string ChangeSetId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string RecordId { get; set; }
        public string RecordChanges { get; set; }
        public int RecordType { get; set; }
        public int Operation { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public RealmChangeSet() { }

        public RealmChangeSet(IChangeSetEntity changeSet)
        {
            ChangeSetIndex = changeSet.ChangeSetIndex;
            ChangeSetId = changeSet.ChangeSetId;
            UserId = changeSet.UserId;
            RecordId = changeSet.RecordId;
            RecordChanges = changeSet.RecordChanges;
            RecordType = changeSet.RecordType;
            Operation = changeSet.Operation;
            CreatedAt = changeSet.CreatedAt;
        }
    }
}
