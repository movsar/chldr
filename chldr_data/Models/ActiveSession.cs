
using chldr_data.Enums;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;

namespace chldr_data.Models
{
    public class ActiveSession
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public SessionStatus Status { get; set; } = SessionStatus.Anonymous;
        public DateTimeOffset AccessTokenExpiresIn { get; set; }
        public UserDto? User { get; set; } = null;
        public ActiveSession() { }
        public void Clear()
        {
            AccessToken = string.Empty;
            RefreshToken = string.Empty;
            Status = SessionStatus.Anonymous;
            AccessTokenExpiresIn = DateTimeOffset.MinValue;
            User = null;
        }
    }
}
