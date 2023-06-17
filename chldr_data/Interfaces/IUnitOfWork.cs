using chldr_data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ISoundsRepository Sounds{ get; }
        IChangeSetsRepository ChangeSets { get; }
        IEntriesRepository Entries { get; }
        ITranslationsRepository Translations { get; }
        ISourcesRepository Sources { get; }
        IUsersRepository Users { get; }
    }
}
