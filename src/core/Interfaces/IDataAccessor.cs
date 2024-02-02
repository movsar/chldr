using core.Interfaces.Repositories;

namespace core.Interfaces
{
    public interface IDataAccessor 
    {
        IPronunciationsRepository Sounds { get; }
        IChangeSetsRepository ChangeSets { get; }
        IEntriesRepository Entries { get; }
        ITranslationsRepository Translations { get; }
        ISourcesRepository Sources { get; }
        IUsersRepository Users { get; }
    }
}
