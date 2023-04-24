using chldr_data.Interfaces;
using Realms;

namespace chldr_data.Entities;
[MapTo("Translation")]
public class RealmTranslation : RealmObject, IEntity
{
    [PrimaryKey]
    public string TranslationId { get; set; } = Guid.NewGuid().ToString();
    public RealmLanguage Language { get; set; } = null!;
    public RealmEntry Entry { get; set; } = null!;
    public RealmUser User { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string RawContents { get; set; } = null!;
    public string? Notes { get; set; }
    public int Rate { get; set; } = 0;
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;
    internal string GetRawContents()
    {
        return Content.ToString();
    }
}
