using domain.DatabaseObjects.Dtos;
using Newtonsoft.Json;
using domain.DatabaseObjects.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace sql_dl.SqlEntities
{
    [Serializable]
    [JsonObject("changeSet")]
    public class SqlChangeSet : IChangeSetEntity
    {
        [Key]
        public long ChangeSetIndex { get; set; }
        public string ChangeSetId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = null!;
        public string RecordId { get; set; } = null!;
        public int RecordType { get; set; }
        public string? RecordChanges { get; set; }
        public int Operation { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
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
