using chldr_data.Interfaces.Repositories;

namespace chldr_data.Interfaces
{
    public interface IUnitOfWork 
    {
        ISoundsRepository Sounds { get; }
        IChangeSetsRepository ChangeSets { get; }
        IEntriesRepository Entries { get; }
        ITranslationsRepository Translations { get; }
        ISourcesRepository Sources { get; }
        IUsersRepository Users { get; }
    }
}
