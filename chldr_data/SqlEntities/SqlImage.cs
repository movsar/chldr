using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;


namespace chldr_data.Entities;
[Table("Image")]
public partial class SqlImage
{
    public string ImageId { get; set; } = Guid.NewGuid().ToString();
    public string? UserId { get; set; }
    public string EntryId { get; set; } = null!;
    public string? FileName { get; set; }
    public int Rate { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public virtual SqlEntry Entry { get; set; } = null!;
    public virtual SqlUser? User { get; set; }
}
