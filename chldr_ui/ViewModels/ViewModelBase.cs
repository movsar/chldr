using chldr_shared.Resources.Localizations;
using chldr_shared.Services;
using chldr_shared.Stores;
using chldr_utils.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.ViewModels
{
    public class ViewModelBase : ComponentBase
    {
        [Inject] internal ContentStore ContentStore { get; set; }
        [Inject] internal UserStore UserStore { get; set; }
        [Inject] internal IStringLocalizer<AppLocalizations> Localizer { get; set; }
        [Inject] internal EnvironmentService? EnvironmentService { get; set; }
        protected async Task CallStateHasChangedAsync()
        {
            await InvokeAsync(() =>
                {
                    StateHasChanged();
                });
        }
    }
}
