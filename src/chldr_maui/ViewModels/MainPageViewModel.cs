using chldr_data.DatabaseObjects.Models;
using dosham.Stores;
using ReactiveUI;
using System.Reactive.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dosham.ViewModels
{
    public class MainPageViewModel : ReactiveObject
    {
        private const int SearchDebounceTime = 250; // Time in milliseconds
        private string _searchText;
        private readonly ObservableAsPropertyHelper<IEnumerable<EntryModel>> _filteredEntries;
        private readonly ContentStore _contentStore;
        private readonly ReactiveCommand<string, IEnumerable<EntryModel>> _searchCommand;

        public MainPageViewModel(ContentStore contentStore)
        {
            _contentStore = contentStore;

            _searchCommand = ReactiveCommand.CreateFromTask<string, IEnumerable<EntryModel>>(SearchEntriesAsync);

            _filteredEntries = this.WhenAnyValue(x => x.SearchText)
                                   .Select(term => term?.Trim())
                                   .DistinctUntilChanged()
                                   .SelectMany(searchTerm =>
                                       string.IsNullOrEmpty(searchTerm)
                                       ? Observable.Return(new List<EntryModel>())
                                       : _searchCommand.Execute(searchTerm).Catch(Observable.Return(new List<EntryModel>())))
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
            if (String.IsNullOrEmpty(searchTerm))
            {
                return new List<EntryModel>();
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
