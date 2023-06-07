using chldr_data.Repositories;

namespace chldr_data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ChangeSetsRepository ChangeSetsRepository { get; }
        WordsRepository WordsRepository { get; }

        void SaveChanges();
    }
}
