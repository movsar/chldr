using chldr_shared.Resources.Localizations;
using chldr_shared.Validators;
using chldr_dataaccess.Services;
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

namespace chldr_shared.ViewModels
{
    public class RegistrationPageViewModel : ComponentBase
    {
        [Inject] RegistrationValidator Validator { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> ErrorMessages { get; } = new();
        public async void SendRegistrationRequest()
        {
            try
            {
                Validator.ValidateAndThrow(this);
            }
            catch (ValidationException ex)
            {
                ErrorMessages.AddRange(ex.Errors.Select(err => err.ErrorMessage));
                return;
            }

            //await ContentStore.RegisterNewUser(Email, Password, Username, FirstName, LastName);
            NavigationManager.NavigateTo("/emailsent");
        }

    }
}
