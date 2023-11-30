using dosham.Stores;
using ReactiveUI;

namespace dosham.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        protected ContentStore ContentStore { get; }
        protected UserStore UserStore { get; }

        protected ViewModelBase()
        {
            ContentStore = GetService<ContentStore>();
            UserStore = GetService<UserStore>();
        }

        protected T GetService<T>() where T : class
        {
            return App.Services.GetRequiredService<T>();
        }
    }
}
