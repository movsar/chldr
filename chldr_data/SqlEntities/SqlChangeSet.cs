using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Interfaces.DatabaseEntities;
using Newtonsoft.Json;

namespace chldr_data.SqlEntities
{
    [Serializable]
    [JsonObject("changeSet")]
    public class SqlChangeSet : IChangeSetEntity
    {
        public long ChangeSetId { get; set; }
        public string UserId { get; set; }
        public string RecordId { get; set; }
        public string RecordChanges { get; set; }
        public int RecordType { get; set; }
        public int Operation { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public virtual SqlUser User { get; set; } = null!;
    }
}
