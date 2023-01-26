using chldr_ui.Resources.Localizations;
using chldr_ui.Services;
using chldr_ui.Stores;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
      
    }
}
