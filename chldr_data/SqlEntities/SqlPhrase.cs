using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Phrase")]

public partial class SqlPhrase
{
    public string PhraseId { get; set; } = Guid.NewGuid().ToString();
    public string EntryId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    public virtual SqlEntry Entry { get; set; } = null!;
}
