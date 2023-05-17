using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;

[Table("Activity")]
public  class SqlActivity
{
    public string ActivityId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = null!;
    public string ObjectId { get; set; } = null!;
    public string ObjectClass { get; set; } = null!;
    public string ObjectProperty { get; set; } = null!;
    public string OldValue { get; set; } = null!;
    public string NewValue { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public virtual SqlUser User { get; set; } = null!;
}
