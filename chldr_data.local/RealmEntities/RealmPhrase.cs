using chldr_data.DatabaseObjects.Interfaces;
using Realms;

namespace chldr_data.local.RealmEntities;
[MapTo("Phrase")]
public class RealmPhrase : RealmObject, IPhraseEntity
{
    [PrimaryKey]
    public string PhraseId { get; set; }
    public RealmEntry Entry { get; set; } = null!;
    public string Content { get; set; } = null!;
    [Ignored]
    public string EntryId => Entry.EntryId;
}
