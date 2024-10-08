﻿using Blazored.Modal;
using Blazored.Modal.Services;
using domain.Resources.Localizations;
using chldr_ui.Components;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Text.RegularExpressions;
using domain.Models;
using domain.Services;
using chldr_app.Stores;
using chldr_application.Services;
using domain.Interfaces;
using domain.Resources.Localizations;

namespace chldr_ui.ViewModels
{
    public class ViewModelBase : ComponentBase
    {
        [Inject] protected JsInteropService JsInterop { get; set; }
        [Inject] protected CultureService CultureService { get; set; }
        [Inject] protected ContentStore ContentStore { get; set; }
        [Inject] protected UserStore UserStore { get; set; }
        [Inject] protected IStringLocalizer<AppLocalizations> Localizer { get; set; }
        [Inject] protected IEnvironmentService? EnvironmentService { get; set; }
        [Inject] protected IExceptionHandler? ExceptionHandler { get; set; }
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
        protected async Task RefreshUiAsync()
        {
            await InvokeAsync(StateHasChanged);
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();

            UserStore.UserStateHasChanged += UserStore_UserStateHasChanged;
            CultureService.CurrentCultureChanged += CultureService_CurrentCultureChanged;
        }

        protected void SetCulture(string cultureCode)
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
            await RefreshUiAsync();
        }

        protected void SetUiLanguage(string cultureCode)
        {
            CultureService.CurrentCulture = cultureCode;
        }
    }
}
