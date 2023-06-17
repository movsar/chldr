using chldr_data.DatabaseObjects.Interfaces;
using Realms;

namespace chldr_data.local.RealmEntities;

[MapTo("Sound")]
public class RealmSound : RealmObject, ISoundEntity
{
    [PrimaryKey] public string SoundId { get; set; }
    public RealmUser User { get; set; } = null!;
    public RealmEntry Entry { get; set; } = null!;
    [Ignored] public string EntryId => Entry.EntryId;
    public string FileName { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
