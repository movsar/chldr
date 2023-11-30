using chldr_data.DatabaseObjects.Models;
using dosham.Stores;
using ReactiveUI;
using System.Text.RegularExpressions;

namespace dosham.ViewModels
{
    public class TranslationViewModel : ViewModelBase
    {
        public TranslationViewModel(ContentStore contentStore, UserStore userStore) : base(contentStore, userStore)
        { }
        private TranslationModel _translation;
        public TranslationModel Translation
        {
            get => _translation;
            set => this.RaiseAndSetIfChanged(ref _translation, value);
        }
        public Action Promote { get; set; }
        public bool IsEditMode { get; set; }

        #region Actions
        public void Upvote() { }
        public void Downvote() { }
        public void Edit()
        {
            IsEditMode = true;
        }
        public void CurrentTranslationSelected()
        {
            var g = 2;
        }

        #endregion

        public async Task DoSearch()
        {
            var translationText = Translation?.Content.ToLower();

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
    }
}
