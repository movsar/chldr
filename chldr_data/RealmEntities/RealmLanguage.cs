using chldr_data.Interfaces.DatabaseEntities;
using Realms;

namespace chldr_data.Entities;
[MapTo("Language")]
public class RealmLanguage : RealmObject, ILanguage
{
    [PrimaryKey]
    public string LanguageId { get; set; } = Guid.NewGuid().ToString();
    public RealmUser? User { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    public IList<RealmTranslation> Translations { get; } 
}
