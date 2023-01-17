using chldr_blazor.Resources.Localizations;
using Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_blazor.ViewModels
{
    internal class RegistrationPageViewModel
    {
        [Inject] private NavigationManager NavigationManager { get; set; }

        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public async void SendRegistrationRequest()
        {
            var dataAccess = App.ServiceProvider.GetService<DataAccess>();


            await dataAccess.RegisterNewUser(Email, Password, Username, FirstName, LastName);
            NavigationManager.NavigateTo("/emailsent");
        }

    }
}
