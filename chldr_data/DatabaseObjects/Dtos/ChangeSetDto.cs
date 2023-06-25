using chldr_data.Enums;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Interfaces;
using Realms;
using chldr_data.Models;
using Newtonsoft.Json;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class ChangeSetDto : IChangeSet
    {
        [PrimaryKey]
        public long ChangeSetIndex { get; set; }
        public string ChangeSetId { get; set; } = Guid.NewGuid().ToString();
        public RecordType RecordType { get; set; }
        public string UserId { get; set; }
        public string RecordId { get; set; }
        public string RecordChanges { get; set; }
        public Operation Operation { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public static ChangeSetDto Create(Operation operation, string userId, RecordType recordType, string recordId, List<Change>? changes = null)
        {
            var changeSet = new ChangeSetDto()
            {
                Operation = operation,
                UserId = userId!,
                RecordId = recordId,
                RecordType = recordType,
            };

            if (changeSet.Operation == Operation.Update && changes != null)
            {
                changeSet.RecordChanges = JsonConvert.SerializeObject(changes);
            }

            return changeSet;
        }
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