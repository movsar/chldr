using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data;
using Data.Interfaces;
using Data.Services;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;

namespace chldr_blazor.ViewModels
{
    [ObservableObject]
    public partial class MainPageViewModel
    {
        [ObservableProperty]
        private List<EntryViewModel> _entries = new();
        private string _lastInputText;

        private DataAccess _dataAccess = App.GetCurrentDataAccess();
        private Stopwatch _stopWatch = new Stopwatch();

        // Fired whenever user types something into the search field
        [ObservableProperty]
        private string _inputText;

        public async void Search(ChangeEventArgs evgentArgs)
        {
            string inputText = evgentArgs.Value.ToString().Replace("1", "Ӏ");

            if (String.IsNullOrWhiteSpace(inputText))
            {
                Entries.Clear();
                return;
            }

            _stopWatch.Restart();
            Entries.Clear();
            await _dataAccess.FindAsync(inputText);
        }

        [ObservableProperty]
        private string statusText;


        internal void ShowRandomEntries()
        {
            var randomEntries = _dataAccess.GetRandomEntries();
            ShowResults(randomEntries);
        }

        public MainPageViewModel()
        {
            // var dataOps = App.ServiceProvider.GetService<DatabaseOperations>();
            // dataOps.RunMaintenance();

            _dataAccess.GotNewSearchResults += DataAccess_GotNewSearchResults;
            ShowRandomEntries();
        }

        private void DataAccess_GotNewSearchResults(SearchResultsModel results)
        {
            var resultsRetrieved = _stopWatch.ElapsedMilliseconds;

            ShowResults(results);

            var resultsShown = _stopWatch.ElapsedMilliseconds;
            Debug.WriteLine($"Found {Entries.Count}");
            _stopWatch.Stop();
        }

        internal void ShowResults(IEnumerable<EntryModel> entries)
        {
            Entries = entries.Select(e => new EntryViewModel(e)).ToList();
        }

        internal void ShowResults(SearchResultsModel searchResults)
        {
            if (_lastInputText != searchResults.InputText)
            {
                Entries.Clear();
            }

            _lastInputText = searchResults.InputText;

            Entries = Entries.Union(searchResults.Entries.Select(em => new EntryViewModel(em))).ToList();
        }
    }
}
