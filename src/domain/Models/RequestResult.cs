using GraphQL;
using Newtonsoft.Json;

namespace domain.Models
{
    public class RequestResult
    {
        public bool Success { get; set; } = false;
        public string? SerializedData { get; set; }
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
        public static T GetData<T>(RequestResult requestResult)
        {
            if (!requestResult.Success || requestResult.SerializedData == null)
            {
                throw new Exception("Error:Unexpected_result");
            }
            return JsonConvert.DeserializeObject<T>(requestResult.SerializedData)!;
        }
    }
}
