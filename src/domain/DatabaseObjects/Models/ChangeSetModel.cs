using domain;
using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Interfaces;
using domain.Enums;

namespace domain.DatabaseObjects.Models
{
    public class ChangeSetModel : IChangeSet
    {
        public ChangeSetModel() { }
        public long ChangeSetIndex { get; set; }
        public string ChangeSetId { get; set; }
        public RecordType RecordType { get; set; }
        public string UserId { get; set; }
        public string RecordId { get; set; }
        public string RecordChanges { get; set; }
        public Operation Operation { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public static ChangeSetModel FromEntity(IChangeSetEntity entity)
        {
            // To be used when retrieving objects from the database

            return new ChangeSetModel()
            {
                ChangeSetIndex = entity.ChangeSetIndex,
                ChangeSetId = entity.ChangeSetId,
                UserId = entity.UserId,
                Operation = (Operation)entity.Operation,
                RecordType = (RecordType)entity.RecordType,
                RecordId = entity.RecordId,
                RecordChanges = entity.RecordChanges,
                CreatedAt = entity.CreatedAt,
            };
        }
    }
}
