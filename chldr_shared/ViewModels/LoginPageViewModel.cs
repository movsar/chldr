﻿using chldr_dataaccess.Interfaces;
using chldr_dataaccess.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.ViewModels
{
    public class LoginPageViewModel : ComponentBase
    {
        #region Fields
        #endregion

        #region Properties
        public string Email { get; set; }
        public string Password { get; set; }
        #endregion

        #region Constructors
        public LoginPageViewModel()
        {
        }
        #endregion

        public void SignInWithGoogle() { }
        public void SignInWithTwitter() { }
        public void SignInWithGitHub() { }
        public void SignInWithFacebook() { }
        public async Task SignInWithEmailPassword()
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
