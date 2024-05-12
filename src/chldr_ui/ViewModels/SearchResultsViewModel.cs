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

            ContentStore.SearchResultsReady += ContentStore_SearchResultsReady; ;

            if (!isInitialized)
            {
                CultureService.CurrentCultureChanged += CultureService_CurrentCultureChanged;
                isInitialized = true;
            }

            return base.OnInitializedAsync();
        }

        private void ContentStore_SearchResultsReady(List<EntryModel> entries)
        {
            if (entries.Count == 0)
            {
                return;
            }

            new Task(async () =>
            {
                Console.WriteLine("GotNewSearchResults");

                //var logger = new ConsoleService("GotNewSearchResults", true);
                //logger.StartSpeedTest();

                // ! Without this the page doesn't refresh
                Entries = null;
                await RefreshUiAsync();
                Entries =
                [
                    .. entries,
                ];
                //logger.StopSpeedTest($"Finished adding entries to the collection");

                //logger.StartSpeedTest();
                await RefreshUiAsync();
                //logger.StopSpeedTest($"Finished rendering");
            }).Start();
        }

        private async void CultureService_CurrentCultureChanged(string cultureCode)
        {
            SetUiLanguage(cultureCode);
            await RefreshUiAsync();
        }
    }
}
