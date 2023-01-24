using chldr_shared.Resources.Localizations;
using chldr_shared.Services;
using chldr_shared.Stores;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.ViewModels
{
    public class ViewModelBase : ComponentBase
    {
        [Inject] protected ContentStore ContentStore { get; set; }
        [Inject] public UserStore UserStore { get; set; }
        [Inject] protected IStringLocalizer<AppLocalizations> Localizer { get; set; }
    }
}
