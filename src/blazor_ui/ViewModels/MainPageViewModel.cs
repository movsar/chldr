using chldr_application.Services;
using domain;
using domain.Enums;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        [Inject] JsInteropService? JsInteropService { get; set; }
        private async Task ShowRandoms()
        {
            if (ContentStore.SearchResults.Count > 0)
            {
                return;
            }

            if (EnvironmentService?.CurrentPlatform != Platforms.Web)
            {
                ContentStore.RequestRandomEntries();
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
        }
    }
}
