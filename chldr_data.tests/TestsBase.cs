
using chldr_data.Repositories;
using chldr_data.Interfaces;
using chldr_data.tests.Services;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.tests
{
    public class TestsBase
    {
        public TestsBase()
        {
            _dataAccess = TestDataFactory.GetTestDataAccess();
            _dataAccess.RemoveAllEntries();
        }

        protected ILocalDbReader _dataAccess;
        protected EntriesRepository<EntryModel> EntriesRepository => (EntriesRepository<EntryModel>)_dataAccess.GetRepository<IEntryEntity>();
        protected WordsRepository WordsRepository => (WordsRepository)_dataAccess.GetRepository<IWordEntity>();
        protected PhrasesRepository PhrasesRepository => (PhrasesRepository)_dataAccess.GetRepository<IPhraseEntity>();
        protected LanguagesRepository LanguagesRepository => (LanguagesRepository)_dataAccess.GetRepository<ILanguageEntity>();
        protected SourcesRepository SourcesRepository => (SourcesRepository)_dataAccess.GetRepository<ISourceEntity>();
        protected UsersRepository UsersRepository => (UsersRepository)_dataAccess.GetRepository<IUserEntity>();

    }
}
