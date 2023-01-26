using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using chldr_data.Models;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;
using System.Windows.Input;
using chldr_shared.Stores;

namespace chldr_ui.ViewModels
{
    public partial class TranslationViewModel : ViewModelBase
    {
        [Parameter]
        public TranslationModel? Translation { get; set; }

        protected override void OnParametersSet()
        {
            // Do whatever is needed to initialize
            base.OnParametersSet();
        }

        #region Actions
        public void Upvote() { }
        public void Downvote() { }
        public void Edit() { }
        public void CurrentTranslationSelected()
        {
            var g = 2;
        }

        #endregion

        public void DoSearch()
        {
            var translationText = Translation.Content.ToLower();

            string[] prefixesToSearch = {
                "см",
                "понуд.? от",
                "потенц.? от",
                "прил.? к"
            };

            foreach (var prefix in prefixesToSearch)
            {

                string pattern = $"(?<={prefix}\\W?\\s?)[1ӀӏА-яA-z]+";
                var match = Regex.Match(translationText, pattern, RegexOptions.CultureInvariant);

                if (match.Success)
                {
                    ContentStore.Search(match.ToString());
                    break;
                }
            }

            ContentStore.Search(translationText);
        }

    }
}
