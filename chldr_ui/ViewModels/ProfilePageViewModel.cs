﻿using chldr_shared.Dto;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.ViewModels
{
    public class ProfilePageViewModel : EditFormViewModelBase<UserInfoDto, UserInfoValidator>
    {
        [Inject] NavigationManager NavigationManager { get; set; }
        public Task ValidateAndSubmitAsync()
        {
            throw new NotImplementedException();
        }

        public async Task LogOutAsync()
        {
            await UserStore.LogOutAsync();
            NavigationManager.NavigateTo("/");
        }
    }
}
