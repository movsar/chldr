using chldr_data.DatabaseObjects.Dtos;
using Realms;
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.local.RealmEntities
{
    [MapTo("ChangeSet")]
    public class RealmChangeSet : RealmObject, IChangeSetEntity
    {
        [PrimaryKey]
        public long ChangeSetIndex { get; set; }
        public string ChangeSetId { get; set; }
        public string UserId { get; set; }
        public string RecordId { get; set; }
        public string RecordChanges { get; set; }
        public int RecordType { get; set; }
        public int Operation { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public RealmChangeSet() { }

        public static IChangeSetEntity FromDto(ChangeSetDto entity)
        {
            return new RealmChangeSet()
            {
                ChangeSetIndex = entity.ChangeSetIndex,
                ChangeSetId = entity.ChangeSetId,
                UserId = entity.UserId,
                Operation = (int)entity.Operation,
                RecordType = (int)entity.RecordType,
                RecordId = entity.RecordId,
                RecordChanges = entity.RecordChanges,
                CreatedAt = entity.CreatedAt,
            };
        }
    }
}
