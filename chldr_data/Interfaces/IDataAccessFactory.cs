namespace chldr_data.Interfaces
{
    public enum DataAccessType
    {
        Synced,
        Offline
    }
    public interface IDataAccessFactory
    {
        IDataAccess GetInstance(DataAccessType dataAccessType);
    }
}
