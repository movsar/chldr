using chldr_data.DatabaseObjects.Models;
using chldr_shared.Stores;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace chldr_ui.ViewModels
{
    public class TranslationViewModel : ViewModelBase
    {
        [Inject] NavigationManager NavigationManager { get; set; }
        [Parameter]
        public TranslationModel? Translation { get; set; }
        public bool IsEditMode { get; set; }
        protected override void OnParametersSet()
        {
            // Do whatever is needed to initialize
            base.OnParametersSet();
        }

        #region Actions
        public void Upvote() { }
        public void Downvote() { }
        public void Edit()
        {
            IsEditMode = true;
            StateHasChanged();
        }
        public void CurrentTranslationSelected()
        {
            var g = 2;
        }

        #endregion

        public void DoSearch()
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
                    ContentStore.Search(match.ToString());
                    return;
                }
            }
        }

    }
}
