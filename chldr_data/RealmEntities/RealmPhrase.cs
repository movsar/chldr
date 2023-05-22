using chldr_data.Interfaces;
using chldr_data.Interfaces.DatabaseEntities;
using Realms;

namespace chldr_data.Entities;
[MapTo("Phrase")]
public class RealmPhrase : RealmObject, IPhraseEntity
{
    [PrimaryKey]
    public string PhraseId { get; set; } =Guid.NewGuid().ToString();
    public RealmEntry Entry { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    [Ignored]
    public string EntryId => Entry.EntryId;
}
