using chldr_data.Interfaces;
using chldr_data.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        #region Fields
        #endregion

        #region Properties
        public string Email { get; set; }
        public string Password { get; set; }
        #endregion

        public void SignInWithGoogle() { }
        public void SignInWithTwitter() { }
        public void SignInWithGitHub() { }
        public void SignInWithFacebook() { }
        public async Task SignInWithEmailPassword()
        {
            try
            {
                await UserStore.LogInEmailPassword(Email, Password);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
