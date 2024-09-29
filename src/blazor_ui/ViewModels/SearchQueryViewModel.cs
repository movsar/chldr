using domain.Models;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace chldr_ui.ViewModels
{
    public class SearchQueryViewModel : ViewModelBase
    {
        internal string? SearchQuery { get; set; }
        internal ElementReference SearchInputReference { get; set; }
        internal void ToggleOnModerationFlag()
        {
            ContentStore.RequestEntriesOnModeration();
            //FiltrationFlags.OnModeration = !FiltrationFlags.OnModeration;
        }

        internal void LoadLatestEntries()
        {
            ContentStore.RequestLatestEntries();
        }
    
        static bool isInitialized = false;
        protected override void OnInitialized()
        {
            base.OnInitialized();

            //if (ContentStore.CachedSearchResult.Entries.Count > 0)
            //{
            //    SearchQuery = ContentStore.CachedSearchResult.SearchQuery;
            //}

            if (!isInitialized)
            {
                CultureService.CurrentCultureChanged += CultureService_CurrentCultureChanged;
                isInitialized = true;
            }
        }
        private async void CultureService_CurrentCultureChanged(string cultureCode)
        {
            SetUiLanguage(cultureCode);
            await RefreshUiAsync();
        }
        // Called when something is typed into search input
        public async Task Search(ChangeEventArgs evgentArgs)
        {
            string? inputText = evgentArgs.Value?.ToString();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return;
            }

            ContentStore.FindEntryDeferred(inputText, null);
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
                ContentStore.RequestRandomEntries();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while showing randoms");
            }
        }
    }
}
