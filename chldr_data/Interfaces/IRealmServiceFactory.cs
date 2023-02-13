using chldr_data.Enums;

namespace chldr_data.Interfaces
{
    public interface IRealmServiceFactory
    {
        DataSourceType CurrentDataSource { get; set; }
        IRealmService GetInstance(DataSourceType dataAccessType);
        IRealmService GetActiveInstance();
    }
}
