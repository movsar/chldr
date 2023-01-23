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

namespace chldr_shared.ViewModels
{
    public class LoginPageViewModel : EditFormViewModel<UserInfoDto, UserInfoValidator>
    {
        #region Properties
        public UserInfoDto UserInfo { get; } = new();
        #endregion

        public void SignInWithGoogle() { }
        public void SignInWithTwitter() { }
        public void SignInWithGitHub() { }
        public void SignInWithFacebook() { }
        public async Task SignInWithEmailPassword()
        {
            await UserStore.LogInEmailPasswordAsync(UserInfo.Email, UserInfo.Password);
        }
        public override async Task ValidateAndSubmit()
        {
            await ValidateAndSubmit(UserInfo, new string[] {
                nameof(UserInfo.Email), nameof(UserInfo.Password)
            }, SignInWithEmailPassword);
        }
    }
}
