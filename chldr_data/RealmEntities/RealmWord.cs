using chldr_data.Interfaces;
using Realms;
namespace chldr_data.Entities;

[MapTo("Word")]
public partial class RealmWord : RealmObject, IEntity
{
    [PrimaryKey]
    public string WordId { get; set; } = Guid.NewGuid().ToString();
    public RealmEntry Entry { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    public int? PartOfSpeech { get; set; }
    public string? AdditionalDetails { get; set; }
}
