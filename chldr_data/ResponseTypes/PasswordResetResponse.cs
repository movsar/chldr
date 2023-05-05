using Newtonsoft.Json;

namespace chldr_data.ResponseTypes
{
    [Serializable]
    public class PasswordResetResponse : MutationResponse
    {
        [JsonProperty("resetToken")]
        public string ResetToken { get; set; } = string.Empty;
    }
}
