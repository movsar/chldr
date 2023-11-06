using chldr_data.DatabaseObjects.Dtos;
using Realms;
using chldr_data.DatabaseObjects.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.realm.RealmEntities
{
    [MapTo("ChangeSet")]
    public class RealmChangeSet : RealmObject, IChangeSetEntity
    {
        [Realms.PrimaryKey]
        public long ChangeSetIndex { get; set; }
        public string ChangeSetId { get; set; }
        public string UserId { get; set; }
        public string RecordId { get; set; }
        public string RecordChanges { get; set; }
        public int RecordType { get; set; }
        public int Operation { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public RealmChangeSet() { }

        public static RealmChangeSet FromDto(ChangeSetDto dto, Realm _dbContext)
        {
            if (string.IsNullOrEmpty(dto.ChangeSetId) || _dbContext == null)
            {
                throw new NullReferenceException();
            }

            // Translation
            var changeSet = _dbContext.Find<RealmChangeSet>(dto.ChangeSetIndex);
            if (changeSet == null)
            {
                changeSet = new RealmChangeSet();
            }

            changeSet.ChangeSetIndex = dto.ChangeSetIndex;
            changeSet.ChangeSetId = dto.ChangeSetId;
            changeSet.UserId = dto.UserId;
            changeSet.Operation = (int)dto.Operation;
            changeSet.RecordType = (int)dto.RecordType;
            changeSet.RecordId = dto.RecordId;
            changeSet.RecordChanges = dto.RecordChanges;
            changeSet.CreatedAt = dto.CreatedAt;
            return changeSet;

        }
    }
}
