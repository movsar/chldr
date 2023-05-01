using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Tokens")]
public partial class SqlToken
{
    public string TokenId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = null!;
    // 0 - unspecified; 1 - refresh; 2 - access
    public int Type { get; set; } = 0;
    public string? Value { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;
    public virtual SqlUser User { get; set; } = null!;
}
