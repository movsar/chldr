using chldr_data.Dto;
using chldr_shared.Stores;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;
using System.Web;

namespace chldr_ui.ViewModels
{
    public class SetNewPasswordPageViewModel : EditFormViewModelBase<UserDto, UserInfoValidator>
    {
        #region Properties
        [Inject] NavigationManager? NavigationManager { get; set; }
        public UserDto UserInfo { get; } = new();
        #endregion

        private async Task UpdatePasswordAsync(string token, string newPassword)
        {
            await UserStore.UpdatePasswordAsync(token, newPassword);
        }

        public async Task ValidateAndSubmitAsync()
        {
            var queryParams = HttpUtility.ParseQueryString(new Uri(NavigationManager!.Uri).Query);
            var token = queryParams.Get("token");

            if (string.IsNullOrWhiteSpace(token))
            {
                NavigationManager?.NavigateTo("/");
                return;
            }

            await ValidateAndSubmitAsync(UserInfo, () => UpdatePasswordAsync(token, UserInfo.Password), new string[] { "Password" });
        }

    }
}