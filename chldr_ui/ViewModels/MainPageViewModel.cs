using chldr_shared;
using chldr_shared.Enums;
using chldr_shared.Stores;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        [Inject] JsInteropService? JsInteropService { get; set; }
        private async Task ShowRandoms()
        {
            if (ContentStore.CachedSearchResult.Entries.Count > 0)
            {
                return;
            }

            if (EnvironmentService?.CurrentPlatform != Platforms.Web)
            {
                await ContentStore.LoadRandomEntries();
            }
            else
            {
                await Task.Delay(500);
                await JsInteropService!.ClickShowRandoms();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await ShowRandoms();
            await base.OnInitializedAsync();

            CultureService.CurrentCulture = "ru-RU";
        }
    }
}
