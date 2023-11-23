using chldr_data.Models;

namespace chldr_data.Interfaces
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
