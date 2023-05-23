using chldr_data.DatabaseObjects.Interfaces;
using Realms;

namespace chldr_data.DatabaseObjects.RealmEntities;


[MapTo("Sound")]
public class RealmSound : RealmObject, ISoundEntity
{
    [PrimaryKey]
    public string SoundId { get; set; }
    public RealmUser User { get; set; } = null!;
    public RealmEntry Entry { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
}
