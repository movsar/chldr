using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data;
using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using chldr_native.Stores;
using chldr_native.Factories;

namespace chldr_native.ViewModels
{
    [ObservableObject]
    public partial class MainPageViewModel : ComponentBase
    {
        #region Properties
        [ObservableProperty]
        private List<EntryViewModelBase> _entryViewModels = new();

        [ObservableProperty]
        private string _inputText;

        [ObservableProperty]
        private string _statusText;
        #endregion

        #region Fields
        private Stopwatch _stopWatch = new Stopwatch();
        public ElementReference entriesListViewReference;
        public ElementReference searchInputReference;
        #endregion

        #region Constructors
        public MainPageViewModel()
        {
            App.ContentStore.CurrentEntriesUpdated += ContentStore_CurrentEntriesUpdated;
            ShowRandoms();
        }
        #endregion

        #region EventHandlers
        // Called whenever there is a change to Entries collection
        private void ContentStore_CurrentEntriesUpdated()
        {
            var resultsRetrieved = _stopWatch.ElapsedMilliseconds;

            ShowResults();

            var resultsShown = _stopWatch.ElapsedMilliseconds;
            Debug.WriteLine($"Found {EntryViewModels.Count}");
            _stopWatch.Stop();
        }
        // Called when something is typed into search input
        public void Search(ChangeEventArgs evgentArgs)
        {
            string inputText = evgentArgs.Value.ToString();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return;
            }

            EntryViewModels.Clear();

            _stopWatch.Restart();
            App.ContentStore.Search(inputText);
        }
        #endregion

        #region Methods
        internal void ShowResults()
        {
            var newEntryViewModels = App.ContentStore.CurrentEntries.Select(e => EntryViewModelFactory.CreateViewModel(e)).ToList();
            EntryViewModels = newEntryViewModels;
        }
        internal void ShowRandoms()
        {
            EntryViewModels.Clear();
            App.ContentStore.LoadRandomEntries();
        }
        #endregion

        #region Overrides
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await searchInputReference.FocusAsync();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            PropertyChanged += async (sender, e) =>
            {
                await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            };
            await base.OnInitializedAsync();
        }
        #endregion
    }
}
