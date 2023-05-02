using chldr_data.Dto;
using chldr_data.Interfaces;
using chldr_data.Models;

namespace chldr_data.ResponseTypes
{
    public class LoginResponse : MutationResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTimeOffset ExpiresIn { get; set; }
        public UserDto User { get; set; }
    }
}
