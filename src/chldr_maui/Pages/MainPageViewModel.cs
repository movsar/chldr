using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_utils.Services;
using dosham.Stores;
using ReactiveUI;
using System.Diagnostics;
using System.IO.Compression;
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

            _contentStore.Initialize();
            _contentStore.SearchResultsReady += EntryService_NewDeferredSearchResult;

            // React to changes in SearchText
            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(SearchDebounceTime))
                .DistinctUntilChanged()
                .Subscribe(searchTerm =>
                {
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        _contentStore.FindEntryDeferred(searchTerm);
                    }
                });
        }

        private void EntryService_NewDeferredSearchResult(List<EntryModel> entries)
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
