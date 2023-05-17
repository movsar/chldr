using Newtonsoft.Json;
using Realms.Sync;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.Entities;
[Table("Tokens")]
public partial class SqlToken
{
    public string TokenId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = null!;
    // See, TokenType enum
    public int? Type { get; set; }
    public string Value { get; set; }
    public DateTime? ExpiresIn { get; set; }
    public DateTime? CreatedAt { get; set; }

    [JsonIgnore]
    public virtual SqlUser User { get; set; } = null!;
}
