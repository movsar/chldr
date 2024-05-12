using core.DatabaseObjects.Models;
using chldr_utils.Services;

namespace chldr_ui.ViewModels
{
    public class SearchResultsViewModel : ViewModelBase
    {
        private string _searchQuery = string.Empty;
        internal List<EntryModel> Entries { get; set; } = new();

        static bool isInitialized = false;
        protected override Task OnInitializedAsync()
        {
            Console.WriteLine("OnInitialized");

            //ContentStore.CachedResultsChanged += ContentStore_CachedResultsChanged;

            //// If no new entries have been requested, show entries from the cache
            //if (ContentStore.CachedSearchResult.Entries.Count > 0 && Entries.Count == 0)
            //{
            //    Entries.AddRange(ContentStore.CachedSearchResult.Entries);
            //}

            if (!isInitialized)
            {
                CultureService.CurrentCultureChanged += CultureService_CurrentCultureChanged;
                isInitialized = true;
            }

            return base.OnInitializedAsync();
        }
        private async void CultureService_CurrentCultureChanged(string cultureCode)
        {
            SetUiLanguage(cultureCode);
            await RefreshUiAsync();
        }

        public void ContentStore_CachedResultsChanged()
        {
            new Task(async () =>
            {
                Console.WriteLine("GotNewSearchResults");

                //var logger = new ConsoleService("GotNewSearchResults", true);
                //logger.StartSpeedTest();

                // ! Without this the page doesn't refresh
                Entries = null;
                await RefreshUiAsync();
                Entries = new List<EntryModel>();

                //logger.StopSpeedTest($"Finished setting up");

                //logger.StartSpeedTest();
                //Entries.AddRange(ContentStore.CachedSearchResult.Entries);
                //logger.StopSpeedTest($"Finished adding entries to the collection");

                //logger.StartSpeedTest();
                await RefreshUiAsync();
                //logger.StopSpeedTest($"Finished rendering");
            }).Start();
        }
    }
}
