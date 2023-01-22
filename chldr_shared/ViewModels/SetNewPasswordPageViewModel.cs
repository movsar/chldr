using chldr_shared.Dto;
using chldr_shared.Interfaces;
using chldr_shared.Stores;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace chldr_shared.ViewModels
{
    public class SetNewPasswordPageViewModel : EditFormViewModel<UserInfoDto, UserInfoValidator>
    {
        #region Properties
        [Inject] NavigationManager? NavigationManager { get; set; }
        public string? Token { get; set; }
        public string? TokenId { get; set; }
        public UserInfoDto UserInfo { get; } = new();
        #endregion

        public async void UpdatePassword()
        {
            var queryParams = HttpUtility.ParseQueryString(new Uri(NavigationManager!.Uri).Query);
            Token = queryParams.Get("token");
            TokenId = queryParams.Get("tokenId");

            if (string.IsNullOrWhiteSpace(Token) || string.IsNullOrWhiteSpace(TokenId))
            {
                NavigationManager?.NavigateTo("/");
                return;
            }

            await ValidateAndSubmit(UserInfo, new string[] { "Password" }, Submit);
        }

        private async Task Submit()
        {
            await UserStore.UpdatePasswordAsync(Token, TokenId, UserInfo.Password);
        }
    }
}