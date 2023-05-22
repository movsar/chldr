using chldr_data.DatabaseObjects.DatabaseEntities;
using Realms;

namespace chldr_data.DatabaseObjects.RealmEntities;
[MapTo("Image")]
public class RealmImage : RealmObject, IEntity
{
    [PrimaryKey]
    public string ImageId { get; set; } = Guid.NewGuid().ToString();
    public RealmUser? User { get; set; }
    public RealmEntry Entry { get; set; } = null!;
    public string? FileName { get; set; }
    public int Rate { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
