using Microsoft.AspNetCore.Components;
namespace chldr_ui.ViewModels
{
    public class NavMenuViewModel : ViewModelBase
    {
        public bool collapseNavMenu = true;
        public string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        public void SetCurrentLanguage(ChangeEventArgs args)
        {
            var newCulture = args.Value?.ToString();
            if (newCulture == null)
            {
                return;
            }

            SetUiLanguage(newCulture);
        }
        public void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
