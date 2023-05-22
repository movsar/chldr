
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.Models
{
    public class ActiveSession
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public SessionStatus Status { get; set; } = SessionStatus.Offline;
        public DateTimeOffset AccessTokenExpiresIn { get; set; }
        public UserModel? User { get; set; } = null;

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
