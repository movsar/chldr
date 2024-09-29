using domain.Interfaces.Repositories;

namespace domain.Interfaces
{
    public interface IDataAccessor 
    {
        ISoundsRepository Sounds { get; }
        IChangeSetsRepository ChangeSets { get; }
        IEntriesRepository Entries { get; }
        ITranslationsRepository Translations { get; }
        ISourcesRepository Sources { get; }
        IUsersRepository Users { get; }
    }
}
