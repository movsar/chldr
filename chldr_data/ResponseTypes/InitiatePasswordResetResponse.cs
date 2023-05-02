using Newtonsoft.Json;

namespace chldr_data.ResponseTypes
{
    [Serializable]
    public class InitiatePasswordResetResponse : MutationResponse
    {
        [JsonProperty("resetToken")]
        public string ResetToken { get; set; } = string.Empty;
    }
}
