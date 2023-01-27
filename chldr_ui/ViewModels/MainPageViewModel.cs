using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using chldr_shared.Stores;
using chldr_shared.Services;
using chldr_shared.Enums;
using chldr_data.Models;
using MailKit.Search;

namespace chldr_ui.ViewModels
{
    public partial class MainPageViewModel : ViewModelBase
    {

        #region Properties & Co.
        [Inject] JsInterop? JsInteropFunctions { get; set; }
        internal ElementReference SearchInputReference { get; set; }
        internal string SearchQuery { get; set; }
        #endregion

        #region Fields
        #endregion

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (ContentStore.CachedSearchResults.Count > 0)
            {
                SearchQuery = ContentStore.CachedSearchResults.TakeLast(1).First().SearchQuery;
            }

            ContentStore.DatabaseInitialized += ContentStore_DatabaseInitialized;
        }

        #region Event Handlers
        private async void ContentStore_DatabaseInitialized()
        {
            if (EnvironmentService?.CurrentPlatform != Platforms.Web)
            {
                ContentStore.LoadRandomEntries();
            }
            else
            {
                await Task.Delay(500);
                await JsInteropFunctions!.ClickShowRandoms();
            }
        }

        // Called when something is typed into search input
        public void Search(ChangeEventArgs evgentArgs)
        {
            string? inputText = evgentArgs.Value?.ToString();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return;
            }

            ContentStore.Search(inputText);
        }
        #endregion

        #region Methods
        public void LoadRandomEntries()
        {
            try
            {
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
            if (!firstRender)
            {
                await SearchInputReference.FocusAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }
        #endregion
    }
}
