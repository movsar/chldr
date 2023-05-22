using chldr_data.Enums;
using chldr_data.DatabaseObjects.DatabaseEntities;
using chldr_data.DatabaseObjects.Dtos;

namespace chldr_data.DatabaseObjects.Models
{
    public class ChangeSetModel : IChangeSet
    {
        public long ChangeSetIndex { get; set; }
        public string ChangeSetId { get; set; }
        public RecordType RecordType { get; set; }
        public string UserId { get; set; }
        public string RecordId { get; set; }
        public string RecordChanges { get; set; }
        public Operation Operation { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        private ChangeSetModel() { }
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

        public static ChangeSetModel FromDto(ChangeSetDto entity)
        {
            // To be used when receiving objects from the API

            return new ChangeSetModel()
            {
                ChangeSetIndex = entity.ChangeSetIndex,
                ChangeSetId = entity.ChangeSetId,
                UserId = entity.UserId,
                Operation = entity.Operation,
                RecordType = entity.RecordType,
                RecordId = entity.RecordId,
                RecordChanges = entity.RecordChanges,
                CreatedAt = entity.CreatedAt,
            };
        }
    }
}
