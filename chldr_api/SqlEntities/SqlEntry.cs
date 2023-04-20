using chldr_data.Interfaces;
using Realms;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Entry")]
public partial class SqlEntry : RealmObject, IEntity
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
    [Ignored] public virtual ICollection<SqlImage> Images { get; set; } = new List<SqlImage>();
    [Ignored] public virtual ICollection<SqlSound> Sounds { get; set; } = new List<SqlSound>();
    [Ignored] public virtual SqlSource Source { get; set; } = null!;
    [Ignored] public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();
    [Ignored] public virtual SqlUser User { get; set; } = null!;
    [Ignored] public virtual SqlText? Text { get; set; }
    [Ignored] public virtual SqlPhrase? Phrase { get; set; }
    [Ignored] public virtual SqlWord? Word { get; set; }

}
