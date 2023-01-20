using chldr_shared.Stores;
using FluentValidation;
using Microsoft.AspNetCore.Components;
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
        [Inject] protected UserStore UserStore { get; set; }
        public List<string> ErrorMessages { get; } = new();
    }
}
