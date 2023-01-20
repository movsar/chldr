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
        public List<string> ErrorMessages { get; } = new();
        public async void SendRegistrationRequest()
        {
            try
            {
                Validator.ValidateAndThrow(this.UserInfo);
            }
            catch (ValidationException ex)
            {
                ErrorMessages.AddRange(ex.Errors.Select(err => err.ErrorMessage));
                return;
            }

            //await ContentStore.RegisterNewUser(Email, Password, Username, FirstName, LastName);
            NavigationManager.NavigateTo("/email-sent");
        }

    }
}
