using chldr_data.DatabaseObjects.Models;
using dosham.Stores;
using ReactiveUI;
using System.Reactive.Linq;

namespace dosham.Pages
{
    public class MainPageViewModel : ReactiveObject
    {
        private const int SearchDebounceTime = 250;
        private string _searchText;
        private readonly ContentStore _contentStore;
        private IEnumerable<EntryModel> _filteredEntries;
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
                        _contentStore.FindEntryDeferred(searchTerm);
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

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }
        public IEnumerable<EntryModel> FilteredEntries
        {
            get => _filteredEntries;
            set => this.RaiseAndSetIfChanged(ref _filteredEntries, value);
        }
    }
}
