using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class TokenDto : IToken
    {
        public string TokenId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string Value { get; set; }
        public int Type { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? ExpiresIn { get; set; }

        public static TokenDto FromModel(TokenModel entity)
        {
            return new TokenDto()
            {
                Type = (int)entity.Type!,
                Value = entity.Value,
                UserId = entity.UserId,
                TokenId = entity.TokenId,
                CreatedAt = entity.CreatedAt,
                ExpiresIn = entity.ExpiresIn
            };
        }
    }
}
