using dosham.Stores;
using ReactiveUI;

namespace dosham.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        protected ContentStore ContentStore { get; }
        protected UserStore UserStore { get; }

        protected ViewModelBase(ContentStore contentStore, UserStore userStore)
        {
            ContentStore = contentStore;
            UserStore = userStore;
        }
    }
}
