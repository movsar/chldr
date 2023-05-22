using chldr_data.DatabaseObjects.Dtos;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;
using System.Web;

namespace chldr_ui.ViewModels
{
    public class LoginPageViewModel : EditFormViewModelBase<UserDto, UserInfoValidator>
    {
        #region Properties
        [Inject] NavigationManager? NavigationManager { get; set; }
        public UserDto UserInfo { get; } = new();
        internal static bool PageInitialized { get; private set; } = false;
        internal static bool EmailConfirmationCompleted { get; private set; } = false;
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
        private async Task ConfirmEmail(string token)
        {
            await UserStore.ConfirmUserAsync(token!);
            EmailConfirmationCompleted = true;
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // Skip the first run (OnInitialized runs twice)
            if (PageInitialized == false)
            {
                PageInitialized = true;
                return;
            }
            // Reset for the page reload
            PageInitialized = false;

            if (EmailConfirmationCompleted == true)
            {
                return;
            }

            // Check whether this page has been opened from the email confirmation link
            var queryParams = HttpUtility.ParseQueryString(new Uri(NavigationManager!.Uri).Query);
            var token = queryParams.Get("token");

            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            await ExecuteSafelyAsync(() => ConfirmEmail(token!));
        }

        public async Task ValidateAndSubmitAsync()
        {
            await ValidateAndSubmitAsync(UserInfo, SignInWithEmailPassword, new string[] {
                nameof(UserInfo.Email), nameof(UserInfo.Password)
            });
        }
    }
}
