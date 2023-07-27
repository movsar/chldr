using chldr_data.Models;

namespace chldr_data.Interfaces
{
    public interface IDataProvider
    {
        public string DefaultUserId { get; }
        event Action? DatabaseInitialized;
        bool IsInitialized { get; set; }
        void Initialize();
        IUnitOfWork CreateUnitOfWork(string userId = null);
        void TruncateDatabase();
    }
}
