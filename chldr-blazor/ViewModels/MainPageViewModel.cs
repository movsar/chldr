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
using chldr_blazor.Stores;

namespace chldr_blazor.ViewModels
{
    [ObservableObject]
    public partial class MainPageViewModel
    {
        #region Properties
        [ObservableProperty]
        private List<EntryViewModel> _entries = new();

        [ObservableProperty]
        private string _inputText;

        [ObservableProperty]
        private string _statusText;
        #endregion

        #region Fields
        private Stopwatch _stopWatch = new Stopwatch();
        private ContentStore _contentStore;
        #endregion
        public void Search(ChangeEventArgs evgentArgs)
        {
            string inputText = evgentArgs.Value.ToString().Replace("1", "Ӏ");

            if (String.IsNullOrWhiteSpace(inputText))
            {
                Entries.Clear();
                return;
            }

            _stopWatch.Restart();
            _contentStore.Search(inputText);
        }

        public MainPageViewModel(ContentStore contentStore)
        {
            _contentStore = contentStore;
            _contentStore.CurrentEntriesUpdated += DataAccess_GotNewSearchResults;
        }

        private void DataAccess_GotNewSearchResults()
        {
            var resultsRetrieved = _stopWatch.ElapsedMilliseconds;

            ShowResults();

            var resultsShown = _stopWatch.ElapsedMilliseconds;
            Debug.WriteLine($"Found {Entries.Count}");
            _stopWatch.Stop();
        }

        internal void ShowResults()
        {
            Entries = _contentStore.CurrentEntries.Select(e => new EntryViewModel(e)).ToList();
        }

    }
}
