using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Data.Interfaces;
using Data.Models;
using MongoDB.Bson;
using Realms.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace chldr_blazor.ViewModels
{
    [ObservableObject]
    public partial class TranslationViewModel
    {
        public void CurrentTranslationSelected()
        {
            var g = 2;
        }
        public ICommand TranslationTextClick { get; } = new Command(() =>
        {
            var g = 2;
        });

        public ICommand Search { get; set; }

        private IDataAccessService _dataAccess;

        public async void DoSearch()
        {
            var translationText = Content.ToLower();
            
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
                    await _dataAccess.FindAsync(match.ToString());
                    break;
                }
            }

            await _dataAccess.FindAsync(translationText);
        }

        public string Content { get; }
        public string LanguageCode { get; }
        public TranslationViewModel()
        {
            Search = new RelayCommand(DoSearch);
        }
        public TranslationViewModel(TranslationModel translation)
        {
            Content = translation.Content;
            LanguageCode = translation.Language.Code;
            Content = translation.Content;

            Search = new RelayCommand(DoSearch);
            _dataAccess = App.ServiceProvider.GetService<IDataAccessService>();
        }
    }
}
