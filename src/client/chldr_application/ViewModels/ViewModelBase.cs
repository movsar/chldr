using chldr_app.Stores;
using ReactiveUI;

namespace chldr_application.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        protected async Task GoToAsync(string path)
        {
            //await Shell.Current.GoToAsync($"//{path}");
        }
    }
}
