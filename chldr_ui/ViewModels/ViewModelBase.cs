using chldr_shared.Resources.Localizations;
using chldr_shared.Stores;
using chldr_utils.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace chldr_ui.ViewModels
{
    public class ViewModelBase : ComponentBase
    {
        [Inject] internal ContentStore ContentStore { get; set; }
        [Inject] internal UserStore UserStore { get; set; }
        [Inject] internal IStringLocalizer<AppLocalizations> Localizer { get; set; }
        [Inject] internal EnvironmentService? EnvironmentService { get; set; }
        protected async Task CallStateHasChangedAsync()
        {
            await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
        }
    }
}
