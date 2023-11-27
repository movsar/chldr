using chldr_data.DatabaseObjects.Models;
using dosham.Stores;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace dosham.ViewModels
{
    public class MainPageViewModel : ReactiveObject
    {
        private const int SearchThrottleTime = 800;
        private string _searchText;
        private readonly ObservableAsPropertyHelper<IEnumerable<EntryModel>> _filteredEntries;
        private readonly ContentStore _contentStore;

        public MainPageViewModel(ContentStore contentStore)
        {
            _contentStore = contentStore;
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
            set
            {
                this.RaiseAndSetIfChanged(ref _searchText, value);
            }
        }

        public IEnumerable<EntryModel> FilteredEntries => _filteredEntries.Value;

        private async Task<IEnumerable<EntryModel>> SearchEntriesAsync(string searchTerm)
        {
            // Replace with actual async search logic
            await Task.Delay(100); // Simulating async operation
            return new List<EntryModel>
            {
                new EntryModel
                {
                    Content = "Sample Entry"
                }
            };
        }
    }
}
