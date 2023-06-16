namespace chldr_data.Interfaces
{
    public interface ISqlUnitOfWork : IUnitOfWork
    {
        void BeginTransaction();
        void Rollback();
        void Commit();
    }
}
