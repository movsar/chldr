using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Sound")]

public partial class SqlSound
{
    public string SoundId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = null!;
    public string EntryId { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public virtual SqlEntry Entry { get; set; } = null!;
    public virtual SqlUser User { get; set; } = null!;
}
