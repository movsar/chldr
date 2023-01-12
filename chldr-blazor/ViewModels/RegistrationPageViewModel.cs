using Data.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_blazor.ViewModels
{
    internal class RegistrationPageViewModel
    {
        [Inject] private NavigationManager NavigationManager { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public async void SendRegistrationRequest()
        {
            var dataAccess = App.GetCurrentDataAccess();
            if (dataAccess is OnlineDataAccess)
            {
                var onlineDataAccess = dataAccess as OnlineDataAccess;
                await onlineDataAccess.RegisterNewUser(Email, Password, Username, FirstName, LastName);
                NavigationManager.NavigateTo("/emailsent");
            }
        }
    }
}
