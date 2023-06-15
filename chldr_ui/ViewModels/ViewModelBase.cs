using Blazored.Modal.Services;
using chldr_data.Resources.Localizations;
using chldr_shared.Stores;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Text.RegularExpressions;

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
        [Inject] internal NavigationManager NavigationManager { get; set; }
        [CascadingParameter] protected IModalService Modal { get; set; } = default!;
        protected async Task RefreshUi()
        {
            await InvokeAsync(StateHasChanged);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            UserStore.UserStateHasChanged += UserStore_UserStateHasChanged;
            CultureService.CurrentCultureChanged += CultureService_CurrentCultureChanged;
        }

        private void CultureService_CurrentCultureChanged(string cultureCode)
        {
            var culture = CultureInfo.GetCultureInfo(cultureCode);

            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        private async void UserStore_UserStateHasChanged()
        {
            await RefreshUi();
        }

        protected void SetUiLanguage(string culture)
        {
            CultureService.CurrentCulture = culture;
        }
    }
}
