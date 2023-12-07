using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using dosham.Stores;
using ReactiveUI;
using System.Text.RegularExpressions;

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

        public EntryViewModel()
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
                    await ContentStore.EntryService.FindAsync(match.ToString());
                    return;
                }
            }
        }
        public bool CanEdit()
        {
            // Anyone should be able to open an entry for edit mode, if they're logged in and active
            // However, they might not be able to change anything, that will be governed by CanEdit* methods
            return UserStore.IsLoggedIn && UserStore.CurrentUser!.Status == UserStatus.Active;
        }

    }
}
