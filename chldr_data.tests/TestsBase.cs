using chldr_data.Models.Words;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_data.Interfaces;
using chldr_data.tests.Services;
using chldr_data.Interfaces.DatabaseEntities;

namespace chldr_data.tests
{
    public class TestsBase
    {
        public TestsBase()
        {
            _dataAccess = TestDataFactory.GetTestDataAccess();
            _dataAccess.RemoveAllEntries();
        }

        protected IDataAccess _dataAccess;
        protected EntriesRepository<EntryModel> EntriesRepository => (EntriesRepository<EntryModel>)_dataAccess.GetRepository<IEntryEntity>();
        protected WordsRepository WordsRepository => (WordsRepository)_dataAccess.GetRepository<IWordEntity>();
        protected PhrasesRepository PhrasesRepository => (PhrasesRepository)_dataAccess.GetRepository<IPhraseEntity>();
        protected LanguagesRepository LanguagesRepository => (LanguagesRepository)_dataAccess.GetRepository<ILanguageEntity>();
        protected SourcesRepository SourcesRepository => (SourcesRepository)_dataAccess.GetRepository<SourceModel>();
        protected UsersRepository UsersRepository => (UsersRepository)_dataAccess.GetRepository<UserModel>();

    }
}
