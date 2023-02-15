using chldr_shared.Stores;
using Microsoft.AspNetCore.Components;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace chldr_ui.ViewModels
{
    public class NavMenuViewModel : ViewModelBase
    {
        public bool collapseNavMenu = true;

        public string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        public void SetCurrentLanguage(ChangeEventArgs args)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(args.Value.ToString());
        }
        public void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
        protected override Task OnParametersSetAsync()
        {
            UserStore.UserStateHasChanged += () =>
            {
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
            };

            return base.OnParametersSetAsync();
        }
    }
}
