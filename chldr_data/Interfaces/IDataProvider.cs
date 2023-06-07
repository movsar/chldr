namespace chldr_data.Interfaces
{
    public interface IDataProvider
    {
        IUnitOfWork CreateUnitOfWork();
    }
}
