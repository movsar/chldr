using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace chldr_data.remote.SqlEntities;
[Table("Tokens")]
public class SqlToken : ITokenEntity
{
    [Key]
    public string TokenId { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = null!;
    public int? Type { get; set; }
    public string Value { get; set; }
    public DateTimeOffset? ExpiresIn { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public virtual SqlUser User { get; set; } = null!;

    internal static SqlToken FromDto(TokenDto dto)
    {
        return new SqlToken()
        {
            Type = dto.Type!,
            Value = dto.Value,
            UserId = dto.UserId,
            TokenId = dto.TokenId,
            CreatedAt = dto.CreatedAt,
            ExpiresIn = dto.ExpiresIn
        };
    }
}
