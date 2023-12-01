using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using dosham.Stores;
using ReactiveUI;

namespace dosham.ViewModels
{
    public class EntryViewModel : ViewModelBase
    {
        #region Properties, Actions and Constructors
        private EntryModel _entry;
        public EntryModel Entry
        {
            get => _entry;
            set => this.RaiseAndSetIfChanged(ref _entry, value);
        }

        public EntryViewModel(ContentStore contentStore, UserStore userStore) : base(contentStore, userStore)
        { }
        #endregion

        public void ListenToPronunciation() { }
        public void NewTranslation() { }
        public void AddToFavorites() { }
        public async Task Remove()
        {
            await ContentStore.EntryService.RemoveAsync(Entry, UserStore.CurrentUser.Id!);
        }
        public void Share() { }
        public async Task PromoteEntryAsync()
        {
            await ContentStore.EntryService.PromoteAsync(Entry, UserStore.CurrentUser);
        }
        public async Task PromoteTranslationAsync(ITranslation translation)
        {
            await ContentStore.EntryService.PromoteTranslationAsync(translation, UserStore.CurrentUser);
        }
        public void Downvote() { }
        public void Flag() { }



        public bool CanEdit()
        {
            // Anyone should be able to open an entry for edit mode, if they're logged in and active
            // However, they might not be able to change anything, that will be governed by CanEdit* methods
            return UserStore.IsLoggedIn && UserStore.CurrentUser!.Status == UserStatus.Active;
        }

    }
}
