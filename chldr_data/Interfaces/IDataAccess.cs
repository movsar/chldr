using chldr_data.Enums;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_data.Services;
using Realms;

namespace chldr_data.Interfaces
{
    public interface IDataAccess
    {
        event Action<DataSourceType> DatasourceInitialized;
        void ActivateDatasource(DataSourceType dataSourceType);
        void RemoveAllEntries();
        EntriesRepository<EntryModel> EntriesRepository { get; }
        UsersRepository UsersRepository { get; }
        PhrasesRepository PhrasesRepository { get; }
        WordsRepository WordsRepository { get; }
        LanguagesRepository LanguagesRepository { get; }
        SourcesRepository SourcesRepository { get; }
    }
}