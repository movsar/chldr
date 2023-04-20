using chldr_data.Interfaces;
using Realms;

namespace chldr_data.Entities;
public  class Entry : RealmObject, IEntity
{
    [PrimaryKey]
    public string EntryId { get; set; } = Guid.NewGuid().ToString();
    public User User { get; set; } = null!;
    public Source Source { get; set; } = null!;
    public int Type { get; set; } = 0;
    public int Rate { get; set; } = 0;
    public string? RawContents { get; set; }
    public Text? Text { get; set; }
    public Phrase? Phrase { get; set; }
    public Word? Word { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public IList<Image> Images { get; } = new List<Image>();
    public IList<Sound> Sounds { get; } = new List<Sound>();
    public IList<Translation> Translations { get; } = new List<Translation>();
}
