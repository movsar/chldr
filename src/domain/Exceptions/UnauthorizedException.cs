namespace domain.Exceptions
{
    public class UnauthorizedException : Exception
    {
        // Default message for the exception
        private const string DefaultMessage = "Error:You_are_not_authorized_for_this_operation";

        public UnauthorizedException() : base(DefaultMessage) { }

        public UnauthorizedException(string message) : base(message) { }

        public UnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
