using chldr_data.DatabaseObjects.Dtos;

namespace chldr_data.ResponseTypes
{
    public class LoginResult : OperationResult
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTimeOffset? AccessTokenExpiresIn { get; set; }
        public UserDto? User { get; set; }
    }
}
