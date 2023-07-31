using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.Enums;
using chldr_data.Interfaces;

namespace chldr_data.DatabaseObjects.Models
{
    public class TokenModel : IToken
    {
        public string TokenId { get; set; }
        public string UserId { get; set; } = null!;
        public string Value { get; set; }
        public TokenType Type { get; set; }
        public DateTimeOffset? ExpiresIn { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }

        public static TokenModel FromEntity(ITokenEntity entity)
        {
            return new TokenModel()
            {
                Type = (TokenType)entity.Type!,
                Value = entity.Value,
                UserId = entity.UserId,
                TokenId = entity.TokenId,
                CreatedAt = entity.CreatedAt,
                ExpiresIn = entity.ExpiresIn
            };
        }
    }
}
