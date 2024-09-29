namespace domain.Interfaces
{
    public interface IExceptionHandler
    {
        Exception Error(Exception ex);
        Exception Error(string message);
        void LogDebug(string message);
        void LogError(Exception ex, string msg = "");
        void LogError(string message);
    }
}
