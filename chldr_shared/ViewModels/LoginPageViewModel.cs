using chldr_data.Interfaces;
using chldr_data.Services;
using chldr_shared.Dto;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace chldr_shared.ViewModels
{
    public class LoginPageViewModel : EditFormViewModel<UserInfoDto, UserInfoValidator>
    {
        #region Properties
        [Inject] NavigationManager? NavigationManager { get; set; }
        public UserInfoDto UserInfo { get; } = new();
        public bool UserActivationCompleted { get; private set; } = false;
        #endregion

        public void SignInWithGoogle() { }
        public void SignInWithTwitter() { }
        public void SignInWithGitHub() { }
        public void SignInWithFacebook() { }
        private async Task SignInWithEmailPassword()
        {
            await UserStore.LogInEmailPasswordAsync(UserInfo.Email, UserInfo.Password);
            NavigationManager!.NavigateTo("/");
        }
        private async Task ConfirmEmail(string token, string tokenId, string email)
        {
            await UserStore.ConfirmUserAsync(token!, tokenId!, email!);
            UserActivationCompleted = true;
            StateHasChanged();
        }

        private async Task HandleEmailConfirmation()
        {
            // Handle confirmation link click
            var queryParams = HttpUtility.ParseQueryString(new Uri(NavigationManager!.Uri).Query);
            var token = queryParams.Get("token");
            var tokenId = queryParams.Get("tokenId");
            var email = queryParams.Get("email");

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(tokenId) || string.IsNullOrWhiteSpace(email))
            {
                return;
            }

            await ExecuteSafelyAsync(() => ConfirmEmail(token!, tokenId!, email!));
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await HandleEmailConfirmation();
        }
        public override async Task ValidateAndSubmit()
        {
            await ValidateAndSubmit(UserInfo, new string[] {
                nameof(UserInfo.Email), nameof(UserInfo.Password)
            }, SignInWithEmailPassword);
        }
    }
}
