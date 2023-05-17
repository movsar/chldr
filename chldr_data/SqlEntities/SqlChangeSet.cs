using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Interfaces.DatabaseEntities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.SqlEntities
{
    [Serializable]
    [JsonObject("changeSet")]
    public class SqlChangeSet : IChangeSet
    {
        [JsonProperty("changeSetId")]
        public long ChangeSetId { get; set; }
        public string UserId { get; set; }
        [NotMapped]
        public string RecordSerialized { get; set; }
        public string RecordId { get; set; }
        public RecordType RecordType { get; set; }
        public Operation Operation { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public virtual SqlUser User { get; set; } = null!;
    }
}
