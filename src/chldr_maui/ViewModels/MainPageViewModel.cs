using chldr_data.DatabaseObjects.Models;
using dosham.Stores;
using ReactiveUI;
using System.Reactive.Linq;

namespace dosham.ViewModels
{
    public class MainPageViewModel : ReactiveObject
    {
        private const int SearchThrottleTime = 250;
        private string _searchText;
        private readonly ObservableAsPropertyHelper<IEnumerable<EntryModel>> _filteredEntries;
        private readonly ContentStore _contentStore;

        public MainPageViewModel(ContentStore contentStore)
        {
            _contentStore = contentStore;

            var searchCommand = ReactiveCommand.CreateFromTask<string, IEnumerable<EntryModel>>(SearchEntriesAsync);

            _filteredEntries = this.WhenAnyValue(x => x.SearchText)
                                  .Throttle(TimeSpan.FromMilliseconds(SearchThrottleTime))
                                  .Select(term => term?.Trim())
                                  .DistinctUntilChanged()
                                  .SelectMany(searchTerm => searchCommand.Execute(searchTerm).Catch(Observable.Return(new List<EntryModel>())))
                                  .ToProperty(this, x => x.FilteredEntries, out _filteredEntries);
        }

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public IEnumerable<EntryModel> FilteredEntries => _filteredEntries.Value;

        private async Task<IEnumerable<EntryModel>> SearchEntriesAsync(string searchTerm)
        {
            if (searchTerm == null)
            {
                return await _contentStore.EntryService.GetRandomsAsync(50);
            }
            var entries = await _contentStore.EntryService.FindAsync(searchTerm);
            return entries;
        }
    }
}
