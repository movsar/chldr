using Blazored.Modal;
using Blazored.Modal.Services;
using chldr_data.Resources.Localizations;
using chldr_shared;
using chldr_shared.Stores;
using chldr_ui.Components;
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
        [Inject] protected CultureService CultureService { get; set; }
        [Inject] protected ContentStore ContentStore { get; set; }
        [Inject] protected UserStore UserStore { get; set; }
        [Inject] protected IStringLocalizer<AppLocalizations> Localizer { get; set; }
        [Inject] protected EnvironmentService? EnvironmentService { get; set; }
        [Inject] protected ExceptionHandler? ExceptionHandler { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [CascadingParameter] protected IModalService Modal { get; set; } = default!;
        protected async Task<bool> AskForConfirmation(string message)
        {
            var parameters = new ModalParameters().Add(nameof(ConfirmationDialog.Message), Localizer[message].ToString());

            var confirmationResult = Modal.Show<ConfirmationDialog>("", parameters, new ModalOptions()
            {
                HideHeader = true,
                Size = ModalSize.Automatic,
                HideCloseButton = true
            });

            var result = await confirmationResult.Result;

            return result.Confirmed;
        }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return base.OnAfterRenderAsync(firstRender);
        }
        protected async Task RefreshUi()
        {
            await InvokeAsync(StateHasChanged);
        }
        static bool isInitialized = false;
        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (!isInitialized)
            {
                UserStore.UserStateHasChanged += UserStore_UserStateHasChanged;
                CultureService.CurrentCultureChanged += CultureService_CurrentCultureChanged;

                isInitialized = true;
            }
        }

        private void SetCulture(string cultureCode)
        {
            var culture = CultureInfo.GetCultureInfo(cultureCode);

            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
        private void CultureService_CurrentCultureChanged(string cultureCode)
        {
            if (Thread.CurrentThread.CurrentUICulture.Name == cultureCode)
            {
                return;
            }

            SetCulture(cultureCode);
        }

        private async void UserStore_UserStateHasChanged()
        {
            await RefreshUi();
        }

        protected void SetUiLanguage(string cultureCode)
        {
            CultureService.CurrentCulture = cultureCode;
        }
    }
}
