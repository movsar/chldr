using chldr_data.Repositories;

namespace chldr_data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IChangeSetsRepository ChangeSets { get; }
        IWordsRepository Words { get; }
        void Commit();
    }
}
