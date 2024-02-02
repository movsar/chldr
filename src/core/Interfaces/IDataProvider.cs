using core.Models;

namespace core.Interfaces
{
    public interface IDataProvider
    {
        event Action? DatabaseInitialized;
        bool IsInitialized { get; set; }
        void Initialize();
        IDataAccessor Repositories(string? userId = null);
        void TruncateDatabase();
    }
}
