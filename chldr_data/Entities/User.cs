using chldr_data.Interfaces;
using Realms;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("User")]

public partial class User : RealmObject, IEntity
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
    [Ignored]    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    [Ignored] public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
    [Ignored] public virtual ICollection<Image> Images { get; set; } = new List<Image>();
    [Ignored] public virtual ICollection<Language> Languages { get; set; } = new List<Language>();
    [Ignored] public virtual ICollection<Query> Queries { get; set; } = new List<Query>();
    [Ignored] public virtual ICollection<Sound> Sounds { get; set; } = new List<Sound>();
    [Ignored] public virtual ICollection<Source> Sources { get; set; } = new List<Source>();
    [Ignored] public virtual ICollection<Translation> Translations { get; set; } = new List<Translation>();
}
