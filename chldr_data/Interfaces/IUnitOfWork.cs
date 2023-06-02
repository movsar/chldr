namespace chldr_data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IChangeSetsRepository ChangeSetsRepository { get; }
        IWordsRepository WordsRepository { get; }

        void SaveChanges();
    }
}
