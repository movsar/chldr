namespace chldr_data.ResponseTypes
{
    public class MutationResponse
    {
        public bool Success { get; set; } = false;
        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                Success = string.IsNullOrWhiteSpace(value);
            }
        }
        public MutationResponse() { }
        public MutationResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
