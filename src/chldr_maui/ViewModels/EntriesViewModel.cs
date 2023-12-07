using chldr_data.DatabaseObjects.Models;
using dosham.Stores;
using ReactiveUI;

namespace dosham.ViewModels
{
    public class EntriesViewModel : ReactiveObject
    {
        private List<EntryModel> _entries;
        public List<EntryModel> Entries
        {
            get => _entries;
            set => this.RaiseAndSetIfChanged(ref _entries, value);
        }
        
        public EntriesViewModel()
        {
            // React to changes in SearchText
            this.WhenAnyValue(x => x.Entries)
                .Subscribe(searchTerm =>
                {
                   
                });
        }
    }
}
