using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.remote.SqlEntities;
[Table("Sound")]

public class SqlSound
{
    public string SoundId { get; set; }
    public string UserId { get; set; } = null!;
    public string EntryId { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public virtual SqlEntry Entry { get; set; } = null!;
    public virtual SqlUser User { get; set; } = null!;
}
