namespace chldr_data.Interfaces
{
    public interface IDataProvider
    {
        event Action? DatabaseInitialized;
        bool IsInitialized { get; set; }
        void Initialize();
        IUnitOfWork CreateUnitOfWork(string userId = null);
        void TruncateDatabase();
    }
}
