using chldr_data.Interfaces.DatabaseEntities;
using Realms;

namespace chldr_data.Entities;
[MapTo("User")]
public class RealmUser : RealmObject, IEntity, IUser
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
    public DateTimeOffset CreatedAt { get; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; } = DateTime.Now;
    public IList<RealmActivity> Activities { get; } 
    public IList<RealmEntry> Entries { get; } 
    public IList<RealmImage> Images { get; }
    public IList<RealmLanguage> Languages { get; } 
    public IList<RealmQuery> Queries { get; }
    public IList<RealmSound> Sounds { get; }
    public IList<RealmSource> Sources { get; }
    public IList<RealmTranslation> Translations { get; }
}
