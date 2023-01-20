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

namespace chldr_shared.ViewModels
{
    public class ResetPasswordPageViewModel : ViewModelBase
    {
        [Inject] UserInfoValidator Validator { get; set; }
        public UserInfoDto UserInfo { get; set; }
        public async void SendPasswordResetRequest()
        {
            var result = Validator.Validate(this.UserInfo);
            if (!result.IsValid)
            {
                ErrorMessages.AddRange(result.Errors.Select(err => err.ErrorMessage));
                return;
            }
            await UserStore.SendPasswordResetRequestAsync(UserInfo.Email);
        }
    }
}
