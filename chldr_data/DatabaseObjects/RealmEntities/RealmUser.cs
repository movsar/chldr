using chldr_data.DatabaseObjects.Interfaces;
using Realms;

namespace chldr_data.DatabaseObjects.RealmEntities;
[MapTo("User")]
public class RealmUser : RealmObject, IUserEntity
{
    [PrimaryKey]
    public string UserId { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public int Rate { get; set; } = 0;
    public string? ImagePath { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }
    public int? IsModerator { get; set; }
    public int? Status { get; set; }
    public DateTimeOffset CreatedAt { get; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; } = DateTime.Now;
    public IList<RealmEntry> Entries { get; }
    public IList<RealmLanguage> Languages { get; }
    public IList<RealmQuery> Queries { get; }
    public IList<RealmSound> Sounds { get; }
    public IList<RealmSource> Sources { get; }
    public IList<RealmTranslation> Translations { get; }
}
