namespace domain.Exceptions
{
    public class InvalidArgumentsException : Exception
    {
        // Default message for the exception
        private const string DefaultMessage = "Error:Invalid_arguments_check_your_input";

        public InvalidArgumentsException() : base(DefaultMessage) { }

        public InvalidArgumentsException(string message) : base(message) { }

        public InvalidArgumentsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
