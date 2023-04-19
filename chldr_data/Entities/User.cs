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
    public sbyte? IsModerator { get; set; }
    public sbyte? AccountStatus { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();
    public virtual ICollection<Language> Languages { get; set; } = new List<Language>();
    public virtual ICollection<Query> Queries { get; set; } = new List<Query>();
    public virtual ICollection<Sound> Sounds { get; set; } = new List<Sound>();
    public virtual ICollection<Source> Sources { get; set; } = new List<Source>();
    public virtual ICollection<Translation> Translations { get; set; } = new List<Translation>();
}
