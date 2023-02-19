using chldr_data.Models;
using chldr_utils.Services;

namespace chldr_ui.ViewModels
{
    public class SearchResultsViewModel : ViewModelBase
    {
        private string _searchQuery = string.Empty;
        internal List<EntryModel> Entries { get; set; } = new();

        protected override void OnInitialized()
        {
            Console.WriteLine("OnInitialized");

            ContentStore.CachedResultsChanged += ContentStore_CachedResultsChanged;
            UserStore.UserStateHasChanged += () => new Task(async () => await CallStateHasChangedAsync());

            // If no new entries have been requested, show entries from the cache
            if (ContentStore.CachedSearchResult.Entries.Count > 0 && Entries.Count == 0)
            {
                Entries.AddRange(ContentStore.CachedSearchResult.Entries);
            }

            base.OnInitialized();
        }

        public void ContentStore_CachedResultsChanged()
        {
            new Task(async () =>
            {
                Console.WriteLine("GotNewSearchResults");

                var logger = new ConsoleService("GotNewSearchResults", true);
                logger.StartSpeedTest();

                // ! Without this the page doesn't refresh
                Entries = null;
                await CallStateHasChangedAsync();
                Entries = new List<EntryModel>();

                logger.StopSpeedTest($"Finished setting up");

                logger.StartSpeedTest();
                Entries.AddRange(ContentStore.CachedSearchResult.Entries);
                logger.StopSpeedTest($"Finished adding entries to the collection");

                logger.StartSpeedTest();
                await CallStateHasChangedAsync();
                logger.StopSpeedTest($"Finished rendering");
            }).Start();
        }
    }
}
