using chldr_data.DatabaseObjects.Interfaces;
using Realms;

namespace chldr_data.DatabaseObjects.RealmEntities;

[MapTo("Entry")]
public class RealmEntry : RealmObject, IEntryEntity
{
    [PrimaryKey]
    public string EntryId { get; set; } = Guid.NewGuid().ToString();
    [Ignored]
    public string? SourceId => Source.SourceId;
    public RealmUser User { get; set; } = null!;
    public RealmSource Source { get; set; } = null!;
    public int Type { get; set; } = 0;
    public int Rate { get; set; } = 0;
    public string RawContents { get; set; } = string.Empty;
    public RealmText? Text { get; set; }
    public RealmPhrase? Phrase { get; set; }
    public RealmWord? Word { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public IList<RealmSound> Sounds { get; }
    public IList<RealmTranslation> Translations { get; }

}
