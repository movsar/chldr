using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces;
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
        private readonly ObservableAsPropertyHelper<IEnumerable<EntryModel>> _filteredEntries;
        private readonly FileService _fileService;
        private readonly ContentStore _contentStore;
        private readonly ReactiveCommand<string, IEnumerable<EntryModel>> _searchCommand;

        public MainPageViewModel(ContentStore contentStore, FileService fileService)
        {
            _fileService = fileService;
            _contentStore = contentStore;

            _contentStore.Initialize();

            _searchCommand = ReactiveCommand.CreateFromTask<string, IEnumerable<EntryModel>>(SearchEntriesAsync);

            _filteredEntries = this.WhenAnyValue(x => x.SearchText)
                                   .Select(term => term?.Trim())
                                   .DistinctUntilChanged()
                                   .SelectMany(searchTerm => _searchCommand.Execute(searchTerm).Catch(Observable.Return(new List<EntryModel>())))
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
                var entries = await _contentStore.EntryService.GetRandomsAsync(50);
                return entries;
            }

            try
            {
                var entries = await _contentStore.EntryService.FindAsync(searchTerm);
                return entries;
            }
            catch
            {
                // Handle or log the exception as needed
                return new List<EntryModel>();
            }
        }

    }
}
