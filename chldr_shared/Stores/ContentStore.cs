using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_utils;
using chldr_utils.Models;
using chldr_utils.Services;
using System.Runtime.CompilerServices;
using chldr_data.ResponseTypes;
using chldr_utils.Interfaces;
using Realms.Sync;
using GraphQL;

namespace chldr_shared.Stores
{
    public class ContentStore
    {

        #region Events
        public event Action? ContentInitialized;
        public event Action? CachedResultsChanged;

        private readonly ExceptionHandler _exceptionHandler;
        private readonly NetworkService _networkService;
        private readonly IDataProvider _dataProvider;
        private readonly ISearchService _searchService;
        private readonly IGraphQLRequestSender _graphQLRequestSender;
        #endregion

        #region Fields and Properties
        private IUnitOfWork _unitOfWork;

        // This shouldn't be normally used, but only to request models that have already been loaded 
        public SearchResultModel CachedSearchResult { get; set; } = new SearchResultModel(new List<EntryModel>());
        public List<LanguageModel> Languages { get; } = new();
        // These are the sources excluding userIds
        #endregion

        #region EventHandlers
        private void DataAccess_GotNewSearchResults(SearchResultModel searchResult)
        {
            if (CachedSearchResult.SearchQuery == searchResult?.SearchQuery)
            {
                CachedSearchResult.Entries.Clear();

                foreach (var entry in searchResult.Entries)
                {
                    CachedSearchResult.Entries.Add(entry);
                }

                CachedResultsChanged?.Invoke();
                return;
            }

            CachedSearchResult = searchResult;
            CachedResultsChanged?.Invoke();
        }
        #endregion

        #region Constructors
        public ContentStore(ExceptionHandler exceptionHandler,
                            NetworkService networkService,
                            IDataProvider dataProvider,
                            ISearchService searchService,
                            IGraphQLRequestSender graphQLRequestSender
            )
        {
            _exceptionHandler = exceptionHandler;
            _networkService = networkService;
            _dataProvider = dataProvider;
            _searchService = searchService;
            _graphQLRequestSender = graphQLRequestSender;
            _searchService.GotNewSearchResult += DataAccess_GotNewSearchResults;

            _dataProvider.EntryUpdated += EntriesRepository_EntryUpdated;
            _dataProvider.DatabaseInitialized += DataAccess_DatasourceInitialized;
            _dataProvider.Initialize();
        }

        private void EntriesRepository_EntryUpdated(EntryModel entry)
        {
            var entryInCachedResults = CachedSearchResult.Entries.First(e => e.EntryId == entry.EntryId);
            if (entryInCachedResults == null)
            {
                return;
            }

            entryInCachedResults = entry;
        }
        #endregion

        #region Methods
        public void Search(string inputText, FiltrationFlags filterationFlags)
        {
            Task.Run(() => _searchService.FindAsync(inputText, filterationFlags));
        }
        public void DeleteEntry(string entryId)
        {
            //EntriesRepository.Delete(entryId);
            //CachedSearchResult.Entries.Remove(CachedSearchResult.Entries.First(e => e.EntryId == entryId));
            //CachedResultsChanged?.Invoke();
        }
        public void LoadRandomEntries()
        {
            CachedSearchResult.Entries.Clear();
            var entries = _searchService.GetRandomEntries();

            CachedSearchResult.Entries.Clear();
            foreach (var entry in entries)
            {
                CachedSearchResult.Entries.Add(entry);
            }

            CachedResultsChanged?.Invoke();
        }
        public void LoadEntriesToFiddleWith()
        {
            CachedSearchResult.Entries.Clear();
            var entries = _searchService.GetWordsToFiddleWith();

            CachedSearchResult.Entries.Clear();
            foreach (var entry in entries)
            {
                CachedSearchResult.Entries.Add(entry);
            }

            CachedResultsChanged?.Invoke();
        }
        public void LoadEntriesOnModeration()
        {
            CachedSearchResult.Entries.Clear();
            var entries = _searchService.GetEntriesOnModeration();

            CachedSearchResult.Entries.Clear();
            foreach (var entry in entries)
            {
                CachedSearchResult.Entries.Add(entry);
            }

            CachedResultsChanged?.Invoke();
        }
        public WordModel GetWordById(string entryId)
        {
            return _unitOfWork.Words.Get(entryId);
        }
        public PhraseModel GetPhraseById(string entryId)
        {
            return _unitOfWork.Phrases.Get(entryId);
        }

        public EntryModel GetEntryById(string entryId)
        {
            var word = _unitOfWork?.Words.GetByEntryId(entryId);
            if (word != null)
            {
                return word;
            }

            var phrase = _unitOfWork?.Phrases.GetByEntryId(entryId);
            if (phrase != null)
            {
                return phrase;
            }

            return null;
        }
        #endregion
        public void DataAccess_DatasourceInitialized()
        {
            _unitOfWork = _dataProvider.CreateUnitOfWork();

            if (Languages.Count == 0)
            {
                Languages.AddRange(_unitOfWork.Languages.GetAllLanguages());
            }

            ContentInitialized?.Invoke();
        }

        public void Search(string query)
        {
            Search(query, new FiltrationFlags());
        }
        public PhraseModel GetCachedPhraseById(string phraseId)
        {
            // Get current Phrase from cached results
            var phrase = CachedSearchResult.Entries
                .Where(e => (EntryType)e.Type == EntryType.Phrase)
                .Cast<PhraseModel>()
                .FirstOrDefault(w => w.PhraseId == phraseId);

            if (phrase == null)
            {
                var npe = new Exception("Error:Phrase_shouldn't_be_null");
                _exceptionHandler.LogAndThrow(npe);
                throw npe;
            }

            return phrase;
        }

        public async Task AddPhrase(IUser userModel, PhraseDto phraseDto)
        {
            // TODO: Send request to insert a new remote phrase
            // _unitOfWork.Phrases.Insert(phraseDto);
        }
        public async Task UpdatePhrase(UserModel loggedInUser, PhraseDto phraseDto)
        {
            // TODO: Send request to update the remote entity
            // _unitOfWork.Phrases.Update(loggedInUser.UserId, phraseDto);
        }
        public async Task UpdateWord(UserModel loggedInUser, WordDto wordDto)
        {
            // Update remote entity
            var request = new GraphQLRequest
            {
                Query = @"
                        mutation updateWord($userId: String!, $wordDto: WordDtoInput!) {
                          updateWord(userId: $userId, wordDto: $wordDto) {
                            success
                            errorMessage
                          }
                        }
                        ",
                // ! The names here must exactly match the names defined in the graphql schema
                Variables = new { loggedInUser.UserId, wordDto }
            };

            var response = await _graphQLRequestSender.SendRequestAsync<MutationResponse>(request, "updateWord");
            if (!response.Data.Success)
            {
                throw new Exception(response.Data.ErrorMessage);
            }

            // Update local entity
             _unitOfWork.Words.Update(wordDto, _unitOfWork.Translations);
        }
    }
}