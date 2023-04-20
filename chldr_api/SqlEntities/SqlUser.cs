using chldr_data.Interfaces;
using Realms;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("User")]

public partial class SqlUser : RealmObject, IEntity
{
    [Realms.PrimaryKey]
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    public string? Email { get; set; }
    public string? Password { get; set; }
    public int Rate { get; set; } = 0;
    public string? ImagePath { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }
    public byte? IsModerator { get; set; }
    public byte? AccountStatus { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    [Ignored]    public virtual ICollection<SqlActivity> Activities { get; set; } = new List<SqlActivity>();
    [Ignored] public virtual ICollection<SqlEntry> Entries { get; set; } = new List<SqlEntry>();
    [Ignored] public virtual ICollection<SqlImage> Images { get; set; } = new List<SqlImage>();
    [Ignored] public virtual ICollection<SqlLanguage> Languages { get; set; } = new List<SqlLanguage>();
    [Ignored] public virtual ICollection<SqlQuery> Queries { get; set; } = new List<SqlQuery>();
    [Ignored] public virtual ICollection<SqlSound> Sounds { get; set; } = new List<SqlSound>();
    [Ignored] public virtual ICollection<SqlSource> Sources { get; set; } = new List<SqlSource>();
    [Ignored] public virtual ICollection<SqlTranslation> Translations { get; set; } = new List<SqlTranslation>();
}
