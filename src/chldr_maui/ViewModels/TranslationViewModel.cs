using chldr_data.DatabaseObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace dosham.ViewModels
{
    public class TranslationViewModel : ViewModelBase
    {
        public TranslationModel? Translation { get; set; }
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
