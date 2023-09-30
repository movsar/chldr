using chldr_data.DatabaseObjects.Dtos;
using chldr_shared.Stores;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;
using System.Web;

namespace chldr_ui.ViewModels
{
    public class SetNewPasswordPageViewModel : EditFormViewModelBase<UserDto, UserInfoValidator>
    {
        #region Properties
        public UserDto UserInfo { get; } = new();
        #endregion

        private async Task UpdatePasswordAsync(string email, string token, string newPassword)
        {
            await UserStore.UpdatePasswordAsync(email, token, newPassword);
        }

        public async Task ValidateAndSubmitAsync()
        {
            var queryParams = HttpUtility.ParseQueryString(new Uri(NavigationManager!.Uri).Query);
            var email = queryParams.Get("email");
            var token = queryParams.Get("token");

            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(token))
            {
                NavigationManager?.NavigateTo("/");
                return;
            }

            await ValidateAndSubmitAsync(UserInfo, () => UpdatePasswordAsync(email, token, UserInfo.Password), new string[] { "Password" });
        }

    }
}