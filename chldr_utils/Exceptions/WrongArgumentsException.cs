namespace chldr_utils.Exceptions
{
    internal class WrongArgumentsException : Exception
    {
        // Default message for the exception
        private const string DefaultMessage = "Error:Wrong_arguments_check_your_input";

        public WrongArgumentsException() : base(DefaultMessage) { }

        public WrongArgumentsException(string message) : base(message) { }

        public WrongArgumentsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
