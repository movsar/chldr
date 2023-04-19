using chldr_data.Interfaces;
using Realms;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Entry")]
public partial class Entry : RealmObject, IEntity
{
    [PrimaryKey]
    public string EntryId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = null!;
    public string SourceId { get; set; } = null!;
    public int Type { get; set; } = 0;
    public int Rate { get; set; } = 0;
    public string? RawContents { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    [Ignored] public virtual ICollection<Image> Images { get; set; } = new List<Image>();
    [Ignored] public virtual ICollection<Sound> Sounds { get; set; } = new List<Sound>();
    [Ignored] public virtual Source Source { get; set; } = null!;
    [Ignored] public virtual ICollection<Translation> Translations { get; set; } = new List<Translation>();
    [Ignored] public virtual User User { get; set; } = null!;
    [Ignored] public virtual Text? Text { get; set; }
    [Ignored] public virtual Phrase? Phrase { get; set; }
    [Ignored] public virtual Word? Word { get; set; }

}
