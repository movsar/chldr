namespace chldr_data.Interfaces
{
    public interface IRealmServiceFactory
    {
        IRealmService GetInstance(DataAccessType dataAccessType);
        IRealmService GetInstance();
    }
}
