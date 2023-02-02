using chldr_shared.Dto;
using chldr_shared.Validators;
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
    public class ResetPasswordPageViewModel : EditFormViewModelBase<UserInfoDto, UserInfoValidator>
    {
        public UserInfoDto UserInfo { get; set; } = new();
        private async Task SendPasswordResetRequest()
        {
            await UserStore.SendPasswordResetRequestAsync(UserInfo.Email);
        }
        public async Task ValidateAndSubmitAsync()
        {
            await ValidateAndSubmitAsync(UserInfo, SendPasswordResetRequest, new string[] { "Email" });
        }
    }
}
