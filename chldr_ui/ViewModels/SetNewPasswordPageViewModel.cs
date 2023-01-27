using chldr_shared.Dto;
using chldr_shared.Stores;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;
using System.Web;

namespace chldr_ui.ViewModels
{
    public class SetNewPasswordPageViewModel : EditFormViewModelBase<UserInfoDto, UserInfoValidator>
    {
        #region Properties
        [Inject] NavigationManager? NavigationManager { get; set; }
        public UserInfoDto UserInfo { get; } = new();
        #endregion

        private async Task UpdatePasswordAsync(string token, string tokenId, string newPassword)
        {
            await UserStore.UpdatePasswordAsync(token, tokenId, newPassword);
        }

        public override async Task ValidateAndSubmit()
        {
            var queryParams = HttpUtility.ParseQueryString(new Uri(NavigationManager!.Uri).Query);
            var token = queryParams.Get("token");
            var tokenId = queryParams.Get("tokenId");

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(tokenId))
            {
                NavigationManager?.NavigateTo("/");
                return;
            }

            await ValidateAndSubmit(UserInfo, new string[] { "Password" }, () => UpdatePasswordAsync(token, tokenId, UserInfo.Password));
        }

    }
}