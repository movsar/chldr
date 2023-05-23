using Newtonsoft.Json;
using Realms.Sync;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.DatabaseObjects.SqlEntities;
[Table("Tokens")]
public partial class SqlToken
{
    // Change when you create a DTO
    public string TokenId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = null!;
    // See, TokenType enum
    public int? Type { get; set; }
    public string Value { get; set; }
    public DateTime? ExpiresIn { get; set; }
    public DateTime? CreatedAt { get; set; }

    public virtual SqlUser User { get; set; } = null!;
}
