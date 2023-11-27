using dosham.Stores;
using ReactiveUI;

namespace dosham.Pages
{
    public class IndexPageViewModel : ReactiveObject
    {
        private readonly ContentStore _contentStore;

        public IndexPageViewModel(ContentStore contentStore)
        {
            _contentStore = contentStore;
        }
    }
}
