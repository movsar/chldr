using chldr_data.Enums;
using chldr_data.Models;
using chldr_data.DatabaseObjects.DatabaseEntities;
using chldr_data.DatabaseObjects.Dtos;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Realms.Sync;

namespace chldr_data.DatabaseObjects.SqlEntities
{
    [Serializable]
    [JsonObject("changeSet")]
    public class SqlChangeSet : IChangeSetEntity
    {
        public long ChangeSetIndex { get; set; }
        public string ChangeSetId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = null!;
        public int Operation { get; set; }
        public string RecordId { get; set; } = null!;
        public int RecordType { get; set; }
        public string RecordChanges { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public virtual SqlUser User { get; set; } = null!;

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
