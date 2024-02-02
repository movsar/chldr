namespace core.Interfaces
{
    public interface IExceptionHandler
    {
        event Action<Exception>? IncomingException;

        Exception Error(Exception ex);
        Exception Error(string message);
        void LogDebug(string message);
        void LogError(Exception ex, string msg = "");
        void LogError(string message);
    }
}
