using chldr_shared;
using chldr_shared.Enums;
using chldr_shared.Stores;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public partial class MainPageViewModel : ViewModelBase
    {
        [Inject] JsInterop? JsInteropFunctions { get; set; }

        private async Task ShowRandoms()
        {
            if (ContentStore.CachedSearchResult.Entries.Count > 0)
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

        protected override async Task OnInitializedAsync()
        {
            await ShowRandoms();
            await base.OnInitializedAsync();
        }
    }
}
