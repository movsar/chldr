using chldr_data.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;

[Table("Phrase")]
public class SqlPhrase : IPhraseEntity
{
    public string PhraseId { get; set; } = Guid.NewGuid().ToString();
    public string EntryId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    [JsonIgnore]
    public virtual SqlEntry Entry { get; set; } = null!;
}
