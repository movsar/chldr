﻿using chldr_shared.Dto;
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
        public UserInfoDto UserInfo { get; } = new();
        #endregion

        private async Task UpdatePasswordAsync(string token, string tokenId, string newPassword)
        {
            await UserStore.UpdatePasswordAsync(token, tokenId, newPassword);
            NavigationManager?.NavigateTo("/login");
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