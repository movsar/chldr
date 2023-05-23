using chldr_data.DatabaseObjects.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.DatabaseObjects.SqlEntities;

[Table("Phrase")]
public class SqlPhrase : IPhraseEntity
{
    public string PhraseId { get; set; }
    public string EntryId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    [JsonIgnore]
    public virtual SqlEntry Entry { get; set; } = null!;
}
