using chldr_utils.Models;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace chldr_ui.ViewModels
{
    public class SearchQueryViewModel : ViewModelBase
    {
        internal string? SearchQuery { get; set; }
        internal ElementReference SearchInputReference { get; set; }
        internal FiltrationFlags FiltrationFlags = new FiltrationFlags();

        internal void ToggleOnModerationFlag()
        {
            ContentStore.LoadEntriesOnModeration();
            //FiltrationFlags.OnModeration = !FiltrationFlags.OnModeration;
        }

        internal void LoadLatestEntries()
        {
            ContentStore.LoadLatestEntries();
        }
        internal void LoadEntriesToFiddleWith()
        {
            ContentStore.LoadEntriesToFiddleWith();
        }
        static bool isInitialized = false;
        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (ContentStore.CachedSearchResult.Entries.Count > 0)
            {
                SearchQuery = ContentStore.CachedSearchResult.SearchQuery;
            }

            CultureService.CurrentCulture = "ru-RU";

            if (!isInitialized)
            {
                CultureService.CurrentCultureChanged += CultureService_CurrentCultureChanged;
                isInitialized = true;
            }
        }
        private async void CultureService_CurrentCultureChanged(string cultureCode)
        {
            SetUiLanguage(cultureCode);
            await RefreshUi();
        }
        // Called when something is typed into search input
        public void Search(ChangeEventArgs evgentArgs)
        {
            string? inputText = evgentArgs.Value?.ToString();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return;
            }

            ContentStore.StartSearch(inputText, FiltrationFlags);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                await SearchInputReference.FocusAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }
        public async void LoadRandomEntries()
        {
            try
            {
                await ContentStore.LoadRandomEntries();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while showing randoms");
            }
        }
    }
}
