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
    public class RegistrationPageViewModel : EditFormViewModel<UserInfoDto, UserInfoValidator>
    {
        [Inject] UserInfoValidator Validator { get; set; }
        public UserInfoDto UserInfo { get; set; } = new();
        public bool FormSubmitted { get; private set; }

        public async void SendRegistrationRequest()
        {
            var result = Validator.Validate(this.UserInfo);
            if (!result.IsValid)
            {
                ErrorMessages = result.Errors.Select(err => err.ErrorMessage).ToList();
                return;
            }

            await ContentStore.RegisterNewUser(UserInfo.Email, UserInfo.Password, UserInfo.Username, UserInfo.FirstName, UserInfo.LastName);
            FormSubmitted = true;
        }

    }
}
