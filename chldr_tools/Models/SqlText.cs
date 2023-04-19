using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_tools.Models;
[Table("Text")]

public partial class SqlText
{
    public string TextId { get; set; } = null!;
    public string EntryId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? Notes { get; set; }
    public virtual SqlEntry Entry { get; set; } = null!;
}
