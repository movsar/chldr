using chldr_data.Interfaces;

namespace chldr_data.Models
{
    public class LoggedInUserStatus
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTimeOffset ExpiresIn { get; set; }
        public IUser User { get; set; }
    }
}
