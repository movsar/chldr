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
        public string? Token { get; set; }
        public string? TokenId { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
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
        private async Task HandleEmailConfirmation()
        {
            // Handle confirmation link click
            var queryParams = HttpUtility.ParseQueryString(new Uri(NavigationManager!.Uri).Query);
            Token = queryParams.Get("token");
            TokenId = queryParams.Get("tokenId");
            UserId = queryParams.Get("userId");
            Email = queryParams.Get("email");

            if (string.IsNullOrWhiteSpace(Token) || string.IsNullOrWhiteSpace(TokenId) || string.IsNullOrWhiteSpace(UserId) || string.IsNullOrWhiteSpace(Email))
            {
                return;
            }

            await UserStore.ConfirmUserAsync(Token, TokenId, UserId, Email);
            UserActivationCompleted = true;

            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await ExecuteSafelyAsync(HandleEmailConfirmation);
        }
        public override async Task ValidateAndSubmit()
        {
            await ValidateAndSubmit(UserInfo, new string[] {
                nameof(UserInfo.Email), nameof(UserInfo.Password)
            }, SignInWithEmailPassword);
        }
    }
}
