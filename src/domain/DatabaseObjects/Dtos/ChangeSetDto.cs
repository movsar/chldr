using domain;
using domain.DatabaseObjects.Models;
using domain.DatabaseObjects.Interfaces;
using domain.Enums;

namespace domain.DatabaseObjects.Dtos
{
    public class ChangeSetDto : IChangeSet
    {
        public long ChangeSetIndex { get; set; }
        public string ChangeSetId { get; set; } = Guid.NewGuid().ToString();
        public RecordType RecordType { get; set; }
        public string UserId { get; set; }
        public string RecordId { get; set; }
        public string RecordChanges { get; set; }
        public Operation Operation { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
      
        public static ChangeSetDto FromModel(ChangeSetModel changeSetModel)
        {
            return new ChangeSetDto()
            {
                ChangeSetIndex = changeSetModel.ChangeSetIndex,
                ChangeSetId = changeSetModel.ChangeSetId,
                UserId = changeSetModel.UserId,
                Operation = changeSetModel.Operation,
                RecordId = changeSetModel.RecordId,
                RecordType = changeSetModel.RecordType,
                RecordChanges = changeSetModel.RecordChanges,
                CreatedAt = changeSetModel.CreatedAt,
            };
        }
    }
}