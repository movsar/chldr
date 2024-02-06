using chldr_app.Stores;
using core.DatabaseObjects.Interfaces;
using core.DatabaseObjects.Models;
using core.Enums;
using ReactiveUI;
using System.Text.RegularExpressions;

namespace chldr_application.ViewModels
{
    public class EntryViewModel : ViewModelBase
    {
        #region Properties, Actions and Constructors
        private EntryModel _entry;
        private readonly ContentStore _contentStore;
        private readonly UserStore _userStore;

        public EntryModel Entry
        {
            get => _entry;
            set => this.RaiseAndSetIfChanged(ref _entry, value);
        }

        public EntryViewModel(ContentStore contentStore, UserStore userStore)
        {
            _contentStore = contentStore;
            _userStore = userStore;
        }
        #endregion

        public void ListenToPronunciation() { }
        public void NewTranslation() { }
        public void AddToFavorites() { }
        public async Task Remove()
        {
            await _contentStore.EntryService.RemoveAsync(Entry, _userStore.CurrentUser.Id!);
        }
        public void Share() { }
        public async Task PromoteEntryAsync()
        {
            await _contentStore.EntryService.PromoteAsync(Entry, _userStore.CurrentUser);
        }
        public async Task PromoteTranslationAsync(ITranslation translation)
        {
            await _contentStore.EntryService.PromoteTranslationAsync(translation, _userStore.CurrentUser);
        }
        public void Downvote() { }
        public void Flag() { }


        #region Actions
        public void Upvote() { }
        public void CurrentTranslationSelected()
        {
            var g = 2;
        }

        #endregion

        public async Task DoSearch()
        {
            var translationText = "";//Translation?.Content.ToLower();

            string[] prefixesToSearch = {
                "см",
                "понуд.? от",
                "потенц.? от",
                "прил.? к",
                "масд.? от"
            };

            foreach (var prefix in prefixesToSearch)
            {

                string pattern = $"(?<={prefix}\\W?\\s?)[1ӀӏА-яA-z]+";
                var match = Regex.Match(translationText, pattern, RegexOptions.CultureInvariant);

                if (match.Success)
                {
                    await _contentStore.EntryService.FindAsync(match.ToString());
                    return;
                }
            }
        }
        public bool CanEdit()
        {
            // Anyone should be able to open an entry for edit mode, if they're logged in and active
            // However, they might not be able to change anything, that will be governed by CanEdit* methods
            return _userStore.IsLoggedIn && _userStore.CurrentUser!.Status == UserStatus.Active;
        }

    }
}
