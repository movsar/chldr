using Newtonsoft.Json;

namespace chldr_data.ResponseTypes
{
    [Serializable]
    public class PasswordResetResult : RequestResult
    {
        [JsonProperty("resetToken")]
        public string ResetToken { get; set; } = string.Empty;
    }
}
