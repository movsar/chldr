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
    public class ResetPasswordPageViewModel : EditFormViewModel<UserInfoDto, UserInfoValidator>
    {
        [Inject] UserInfoValidator Validator { get; set; }
        public UserInfoDto UserInfo { get; set; } = new();
        public bool FormSubmitted { get; set; } = false;
        public bool FormDisabled { get; set; } = false;
        public async void SendPasswordResetRequest()
        {
            // Form validation
            var result = Validator.Validate(this.UserInfo);
            if (!result.IsValid)
            {
                ErrorMessages.AddRange(result.Errors.Select(err => err.ErrorMessage));
                return;
            }

            // Block controls while processing
            FormDisabled = true;

            // Act
            try
            {
                await UserStore.SendPasswordResetRequestAsync(UserInfo.Email);
            }
            catch (Exception ex)
            {
                FormDisabled = false;
                ErrorMessages.Add(ex.Message);
            }

            // Notify of success
            FormSubmitted = true;
            StateHasChanged();
        }
    }
}
