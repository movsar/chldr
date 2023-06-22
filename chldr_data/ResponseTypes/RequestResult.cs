namespace chldr_data.ResponseTypes
{
    public class RequestResult
    {
        public bool Success { get; set; } = false;
        public string SerializedData { get; set; }

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
        public RequestResult() { }
        public RequestResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
