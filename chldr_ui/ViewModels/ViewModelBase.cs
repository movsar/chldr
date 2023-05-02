using chldr_data.Resources.Localizations;
using chldr_shared.Stores;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace chldr_ui.ViewModels
{
    public class ViewModelBase : ComponentBase
    {
        [Inject] internal CultureService CultureService { get; set; }
        [Inject] internal ContentStore ContentStore { get; set; }
        [Inject] internal UserStore UserStore { get; set; }
        [Inject] internal IStringLocalizer<AppLocalizations> Localizer { get; set; }
        [Inject] internal EnvironmentService? EnvironmentService { get; set; }
        [Inject] internal ExceptionHandler? ExceptionHandler { get; set; }
        protected async Task CallStateHasChangedAsync()
        {
            await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            CultureService.CurrentCultureChanged += ViewModel_CurrentCultureChanged;
            UserStore.UserStateHasChanged += UserStore_UserStateHasChanged;
        }

        private async void UserStore_UserStateHasChanged()
        {
            await CallStateHasChangedAsync();
        }

        private async void ViewModel_CurrentCultureChanged(string culture)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            await CallStateHasChangedAsync();
        }

        protected void OnCultureChanged(string culture)
        {
            CultureService.CurrentCulture = culture;
        }
    }
}
