using chldr_data.Interfaces.DatabaseEntities;
using Realms;

namespace chldr_data.Entities;
[MapTo("Entry")]
public class RealmEntry : RealmObject, IEntity
{
    [PrimaryKey]
    public string EntryId { get; set; } = Guid.NewGuid().ToString();
    public RealmUser User { get; set; } = null!;
    public RealmSource Source { get; set; } = null!;
    public int Type { get; set; } = 0;
    public int Rate { get; set; } = 0;
    public string? RawContents { get; set; }
    public RealmText? Text { get; set; }
    public RealmPhrase? Phrase { get; set; }
    public RealmWord? Word { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public IList<RealmImage> Images { get; }
    public IList<RealmSound> Sounds { get; }
    public IList<RealmTranslation> Translations { get; }
}
