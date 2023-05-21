﻿using chldr_data.Dto;
using chldr_data.Enums;
using chldr_data.Interfaces.DatabaseEntities;

namespace chldr_data.Models
{
    public class ChangeSetModel : IChangeSet
    {
        public long ChangeSetIndex { get; }
        public string ChangeSetId { get; set; }
        public RecordType RecordType { get; set; }
        public string UserId { get; set; }
        public string RecordId { get; set; }
        public string RecordChanges { get; set; }
        public Operation Operation { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public ChangeSetModel(ChangeSetDto c)
        {
            ChangeSetIndex = c.ChangeSetIndex;
            ChangeSetId = c.ChangeSetId;
            UserId = c.UserId;
            Operation = c.Operation;
            RecordId = c.RecordId;
            RecordType = c.RecordType;
            RecordChanges = c.RecordChanges;
            CreatedAt = c.CreatedAt;
        }
        public ChangeSetModel(IChangeSetEntity changeSetEntity)
        {
            ChangeSetIndex = changeSetEntity.ChangeSetIndex;
            ChangeSetId = changeSetEntity.ChangeSetId;
            UserId = changeSetEntity.UserId;
            Operation = (Operation)changeSetEntity.Operation;
            RecordId = changeSetEntity.RecordId;
            RecordType = (RecordType)changeSetEntity.RecordType;
            RecordChanges = changeSetEntity.RecordChanges;
            CreatedAt = changeSetEntity.CreatedAt;
        }
    }
}
