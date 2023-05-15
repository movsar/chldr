using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Interfaces.DatabaseEntities;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.SqlEntities
{
    public class SqlChangeSet : IChangeSet
    {
        public long ChangeSetId { get; set; }
        public string UserId { get; set; }
        public string RecordId { get; set; }
        public RecordType RecordType { get; set; }
        public Operation Operation { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
        public virtual SqlUser User { get; set; } = null!;
    }
}
