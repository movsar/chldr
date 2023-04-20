using Realms;
using System.ComponentModel.DataAnnotations.Schema;
using chldr_data.Interfaces;

namespace chldr_data.Entities;

[Table("Activity")]
public partial class SqlActivity : RealmObject, IEntity
{
    [PrimaryKey]
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
    [Ignored] public virtual SqlUser User { get; set; } = null!;
}
