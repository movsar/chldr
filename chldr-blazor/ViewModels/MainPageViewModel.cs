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
        #endregion
     
        #region Constructors
        public MainPageViewModel()
        {
            App.ContentStore.CurrentEntriesUpdated += ContentStore_CurrentEntriesUpdated;
            App.ContentStore.LoadRandomEntries();
        }
        #endregion

        #region EventHandlers
        // Called whenever there is a change to Entries collection
        private void ContentStore_CurrentEntriesUpdated()
        {
            var resultsRetrieved = _stopWatch.ElapsedMilliseconds;

            ShowResults();

            var resultsShown = _stopWatch.ElapsedMilliseconds;
            Debug.WriteLine($"Found {Entries.Count}");
            _stopWatch.Stop();
        }
        // Called when something is typed into search input
        public void Search(ChangeEventArgs evgentArgs)
        {
            string inputText = evgentArgs.Value.ToString();
            if (String.IsNullOrWhiteSpace(inputText))
            {
                Entries.Clear();
                return;
            }

            _stopWatch.Restart();
            App.ContentStore.Search(inputText);
        }
        #endregion

        #region Methods
        internal void ShowResults()
        {
            var newEntries = App.ContentStore.CurrentEntries.Select(e => new EntryViewModel(e)).ToList();
            Entries = newEntries;
        }
        #endregion
    }
}
