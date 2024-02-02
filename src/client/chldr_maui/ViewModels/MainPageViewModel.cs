using chldr_app.Stores;
using core.DatabaseObjects.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;

namespace dosham.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        [Reactive] public string SearchText { get; set; }
        [Reactive] public IEnumerable<EntryModel> FilteredEntries { get; set; }
        
        private const int SearchDebounceTime = 250;
        private readonly ContentStore _contentStore;
        public MainPageViewModel(ContentStore contentStore)
        {
            _contentStore = contentStore;
            _contentStore.SearchResultsReady += OnNewSearchResults;
            _contentStore.ContentInitialized += OnContentInitialized;
            _contentStore.Initialize();

            // React to changes in SearchText
            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(SearchDebounceTime))
                .Select(searchTerm => searchTerm?.Trim())
                .DistinctUntilChanged()
                .Subscribe(searchTerm =>
                {
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        _contentStore.FindEntryDeferred(searchTerm, null);
                    }
                });
        }

        private void OnContentInitialized()
        {
            _contentStore.RequestRandomEntries();
        }

        private void OnNewSearchResults(List<EntryModel> entries)
        {
            FilteredEntries = entries;
        }
    }
}
