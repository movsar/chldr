using chldr_data.DatabaseObjects.DatabaseEntities;
using Realms;
namespace chldr_data.DatabaseObjects.RealmEntities;

[MapTo("Word")]
public class RealmWord : RealmObject, IWordEntity
{
    [PrimaryKey]
    public string WordId { get; set; } = Guid.NewGuid().ToString();
    public RealmEntry Entry { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    public int? PartOfSpeech { get; set; }
    public string? AdditionalDetails { get; set; }
}
