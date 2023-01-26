using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using chldr_shared.Stores;
using chldr_ui.Factories;
using chldr_shared.Services;
using chldr_shared.Enums;

namespace chldr_ui.ViewModels
{
    [ObservableObject]
    public partial class MainPageViewModel : ViewModelBase
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
        private volatile bool _databaseInitialized;
        private bool firstTimeRendered = true;
        #endregion
        [Inject] JsInterop? JsInteropFunctions { get; set; }
        [Inject] EnvironmentService? EnvironmentService { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ContentStore.DatabaseInitialized += MyContentStore_DatabaseInitialized;
            ContentStore.CurrentEntriesUpdated += MyContentStore_CurrentEntriesUpdated;
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (EnvironmentService!.CurrentPlatform == Platforms.Web && firstTimeRendered)
            {
                firstTimeRendered = false;
                await Task.Delay(500);
                await JsInteropFunctions!.ClickShowRandoms();
            }
        }

        private void MyContentStore_DatabaseInitialized()
        {
            _databaseInitialized = true;
            if (EnvironmentService!.CurrentPlatform != Platforms.Web)
            {
                firstTimeRendered = false;
                ShowRandoms();
            }
        }

        #region EventHandlers
        // Called whenever there is a change to Entries collection
        private void MyContentStore_CurrentEntriesUpdated()
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
            string? inputText = evgentArgs.Value?.ToString();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return;
            }

            EntryViewModels.Clear();

            _stopWatch.Restart();
            ContentStore.Search(inputText);
        }
        #endregion

        #region Methods
        public void ShowResults()
        {
            var newEntryViewModels = ContentStore.CurrentEntries.Select(e => EntryViewModelFactory.CreateViewModel(e)).ToList();
            EntryViewModels = newEntryViewModels;
        }
        public void ShowRandoms()
        {
            try
            {
                EntryViewModels.Clear();
                ContentStore.LoadRandomEntries();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while showing randoms");
            }
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
