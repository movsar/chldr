namespace chldr_data.ResponseTypes
{
    public class MutationResponse
    {
        public bool Success { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
