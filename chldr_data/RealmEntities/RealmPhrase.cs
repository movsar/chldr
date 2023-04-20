using chldr_data.Interfaces;
using Realms;

namespace chldr_data.Entities;
[MapTo("Phrase")]
internal class RealmPhrase : RealmObject, IEntity
{
    [PrimaryKey]
    public string PhraseId { get; set; } =Guid.NewGuid().ToString();
    public RealmEntry Entry { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
}
