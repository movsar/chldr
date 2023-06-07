using chldr_data.DatabaseObjects.Interfaces;
using Realms;
namespace chldr_data.local.RealmEntities;

[MapTo("Word")]
public class RealmWord : RealmObject, IWordEntity
{
    [PrimaryKey]
    public string WordId { get; set; }
    public RealmEntry Entry { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    public int? PartOfSpeech { get; set; }
    public string? AdditionalDetails { get; set; }
}
