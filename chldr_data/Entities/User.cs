using chldr_data.Interfaces;
using Realms;

namespace chldr_data.Entities;
public  class User : RealmObject, IEntity
{
    [PrimaryKey]
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
    public IList<Activity> Activities { get; set; } = new List<Activity>();
    public IList<Entry> Entries { get; set; } = new List<Entry>();
    public IList<Image> Images { get; set; } = new List<Image>();
    public IList<Language> Languages { get; set; } = new List<Language>();
    public IList<Query> Queries { get; set; } = new List<Query>();
    public IList<Sound> Sounds { get; set; } = new List<Sound>();
    public IList<Source> Sources { get; set; } = new List<Source>();
    public IList<Translation> Translations { get; set; } = new List<Translation>();
}
