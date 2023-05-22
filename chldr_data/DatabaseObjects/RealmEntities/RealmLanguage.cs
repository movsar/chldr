using chldr_data.DatabaseObjects.Interfaces;
using Realms;

namespace chldr_data.DatabaseObjects.RealmEntities;
[MapTo("Language")]
public class RealmLanguage : RealmObject, ILanguageEntity
{
    [PrimaryKey]
    public string LanguageId { get; } = Guid.NewGuid().ToString();
    public RealmUser? User { get; set; }
    [Ignored]
    public string? UserId => User?.UserId;
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public IList<RealmTranslation> Translations { get; }

}
