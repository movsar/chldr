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
        [Inject] JsInterop? JsInteropFunctions { get; set; }


        private async Task ShowRandoms()
        {
            if (ContentStore.CachedSearchResults.Count > 0)
            {
                return;
            }

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

        protected override void OnInitialized()
        {
            ContentStore.DatabaseInitialized += ContentStore_DatabaseInitialized; ;
            base.OnInitialized();
        }

        private void ContentStore_DatabaseInitialized()
        {
            ShowRandoms().Start();
        }
    }
}
