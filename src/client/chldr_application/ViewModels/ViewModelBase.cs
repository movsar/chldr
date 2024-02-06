using chldr_app.Stores;
using ReactiveUI;

namespace chldr_application.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        protected ContentStore ContentStore { get; }
        protected UserStore UserStore { get; }

        protected ViewModelBase()
        {
            //ContentStore = App.Services.GetRequiredService<ContentStore>();
            //UserStore = App.Services.GetRequiredService<UserStore>();
        }

        protected async Task GoToAsync(string path)
        {
            //await Shell.Current.GoToAsync($"//{path}");
        }
    }
}
