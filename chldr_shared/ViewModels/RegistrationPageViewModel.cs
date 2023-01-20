using chldr_shared.Resources.Localizations;
using chldr_shared.Validators;
using chldr_data.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidationException = FluentValidation.ValidationException;
using chldr_shared.Dto;

namespace chldr_shared.ViewModels
{
    public class RegistrationPageViewModel : ViewModelBase
    {
        [Inject] UserInfoValidator Validator { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        public UserInfoDto UserInfo { get; set; }
        public async void SendRegistrationRequest()
        {
            var result = Validator.Validate(this.UserInfo);
            if (!result.IsValid)
            {
                ErrorMessages.AddRange(result.Errors.Select(err => err.ErrorMessage));
                return;
            }

            await ContentStore.RegisterNewUser(UserInfo.Email, UserInfo.Password, UserInfo.Username, UserInfo.FirstName, UserInfo.LastName);
            NavigationManager.NavigateTo("/email-sent");
        }

    }
}
