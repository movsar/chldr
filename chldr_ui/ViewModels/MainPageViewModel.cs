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
            ContentStore.ContentInitialized += ContentStore_ContentInitialized; ;
            base.OnInitialized();
        }

        private async void ContentStore_ContentInitialized()
        {
            await ShowRandoms();
        }
    }
}
