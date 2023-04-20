using chldr_data.Interfaces;
using Realms;
using System.ComponentModel.DataAnnotations.Schema;

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
    IList<Image> Images { get; set; } = new List<Image>();
    IList<Sound> Sounds { get; set; } = new List<Sound>();
    IList<Translation> Translations { get; set; } = new List<Translation>();
}
