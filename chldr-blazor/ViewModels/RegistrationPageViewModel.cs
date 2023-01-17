using chldr_blazor.Resources.Localizations;
using chldr_blazor.Validators;
using Data.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Maui.Controls.Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_blazor.ViewModels
{
    public class RegistrationPageViewModel : ComponentBase
    {
        //public RegistrationPageViewModel(RegistrationValidator Validator)
        //{

        //}
        [Inject] RegistrationValidator Validator { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public async void SendRegistrationRequest()
        {
            try
            {
                Validator.ValidateAndThrow(this);
            }
            catch (Exception ex)
            {
                AlertDialog ad = new AlertDialog() { Content = ex.Message };
                ad.ShowAsync();
            }

            var dataAccess = App.ServiceProvider.GetService<DataAccess>();


            await dataAccess.RegisterNewUser(Email, Password, Username, FirstName, LastName);
            // NavigationManager.NavigateTo("/emailsent");
        }

    }
}
