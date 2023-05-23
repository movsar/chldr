using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.SqlEntities;
using Realms;

namespace chldr_data.DatabaseObjects.RealmEntities
{
    [MapTo("ChangeSet")]
    public class RealmChangeSet : RealmObject, IChangeSetEntity, IEntity
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
            return new SqlChangeSet()
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
