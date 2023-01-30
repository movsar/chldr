using chldr_data.Models;
using chldr_utils;
using chldr_utils.Services;

namespace chldr_ui.ViewModels
{
    public class SearchResultsViewModel : ViewModelBase
    {
        private string _searchQuery = string.Empty;
        public List<EntryModel> Entries { get; } = new();

        protected override void OnInitialized()
        {
            Console.WriteLine("OnInitialized");

            ContentStore.GotNewSearchResult += ContentStore_GotNewSearchResult;
            UserStore.UserStateHasChanged += UserStore_UserStateHasChanged;

            // If no new entries have been requested, show entries from the cache
            if (ContentStore.CachedSearchResults.Count > 0 && Entries.Count == 0)
            {
                Entries.AddRange(ContentStore.CachedSearchResults.SelectMany(sr => sr.Entries));
            }

            base.OnInitialized();
        }

        private void UserStore_UserStateHasChanged()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        private async void ContentStore_GotNewSearchResult(SearchResultModel searchResult)
        {
            Console.WriteLine("GotNewSearchResults");

            var logger = new ConsoleService("GotNewSearchResults", true);
            logger.StartSpeedTest();

            if (_searchQuery == null || !_searchQuery.Equals(searchResult.SearchQuery))
            {
                Entries.Clear();
                _searchQuery = searchResult.SearchQuery;

                await CallStateHasChangedAsync();
                logger.StopSpeedTest($"Finished setting up");
            }

            logger.StartSpeedTest();
            Entries.AddRange(searchResult.Entries);
            logger.StopSpeedTest($"Finished adding entries to the collection");

            logger.StartSpeedTest();
            await CallStateHasChangedAsync();
            logger.StopSpeedTest($"Finished rendering");
        }
    }
}
