using Realms;
using chldr_data.Interfaces.DatabaseEntities;

namespace chldr_data.Entities;
[MapTo("Activity")]
public class RealmActivity : RealmObject, IEntity
{
    [PrimaryKey]
    public string ActivityId { get; set; } = Guid.NewGuid().ToString();
    public RealmUser User { get; set; } = null!;
    public string ObjectId { get; set; } = null!;
    public string ObjectClass { get; set; } = null!;
    public string ObjectProperty { get; set; } = null!;
    public string OldValue { get; set; } = null!;
    public string NewValue { get; set; } = null!;
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
}
