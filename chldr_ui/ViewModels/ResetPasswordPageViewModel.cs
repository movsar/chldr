using chldr_ui.Dto;
using chldr_ui.Validators;
using Microsoft.AspNetCore.Components;
using Realms.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.ViewModels
{
    public class ResetPasswordPageViewModel : EditFormViewModel<UserInfoDto, UserInfoValidator>
    {
        public UserInfoDto UserInfo { get; set; } = new();
        private async Task SendPasswordResetRequest()
        {
            await UserStore.SendPasswordResetRequestAsync(UserInfo.Email);
        }
        public override async Task ValidateAndSubmit()
        {
            await ValidateAndSubmit(UserInfo, new string[] { "Email" }, SendPasswordResetRequest);
        }
    }
}
