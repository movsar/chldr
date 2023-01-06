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
    public partial class IndexViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<EntryViewModel> _entries = new();
        private string _lastInputText;

        private RealmDataAccessService _dataAccess;
        private Stopwatch _stopWatch = new Stopwatch();

        // Fired whenever user types something into the search field
        [ObservableProperty]
        private string _inputText;

        public async void Search(ChangeEventArgs evgentArgs)
        {
            string inputText = evgentArgs.Value.ToString().Replace("1", "Ӏ");

            if (String.IsNullOrWhiteSpace(inputText))
            {
                return;
            }

            _stopWatch.Restart();
            Entries.Clear();
            await _dataAccess.FindAsync(inputText);
        }

        [ObservableProperty]
        private string statusText;

        private void OnNewSearchResults(string inputText, SearchResultsModel searchResults)
        {
            var resultsRetrieved = _stopWatch.ElapsedMilliseconds;

            ShowResults(searchResults, inputText);

            var resultsShown = _stopWatch.ElapsedMilliseconds;
            Debug.WriteLine($"Found {Entries.Count}");
            _stopWatch.Stop();
        }

        internal void ShowRandomEntries()
        {
            Task.Run(() =>
            {
                Entries.Clear();

                var randomEntries = _dataAccess.GetRandomEntries();
                var searchResults = new SearchResultsModel(SearchResultsModel.Mode.Random);
                searchResults.Entries.AddRange(randomEntries);
                ShowResults(searchResults, "");
            });
        }

        public IndexViewModel(IDataAccessService dataAccess)
        {
            _dataAccess = (RealmDataAccessService)dataAccess;
            _dataAccess.NewSearchResults += OnNewSearchResults;
            _dataAccess.DoDangerousTheStuff();

            ShowRandomEntries();
        }

        internal void ShowResults(SearchResultsModel results, string inputText)
        {
            if (_lastInputText != inputText)
            {
                Entries.Clear();
            }

            _lastInputText = inputText;

            foreach (var entry in results.Entries)
            {
                Entries.Add(new EntryViewModel(entry));
            }

            NotifyOfChanges();
        }

    }
}
