using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Services;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using Realms.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using chldr_shared.Stores;

namespace chldr_shared.ViewModels
{
    [ObservableObject]
    public partial class TranslationViewModel : ComponentBase
    {

        [Inject] ContentStore ContentStore { get; set; }
        #region Properties
        [Parameter]
        public string EntityId { get; set; }
        [Parameter]
        public TranslationModel Model { get; set; }
        public ICommand Search { get; set; }
        public string Content { get; set; }
        public string Notes { get; set; }
        public string LanguageCode { get; set; }

        #endregion

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
                    ContentStore.Search(match.ToString());
                    break;
                }
            }

            ContentStore.Search(translationText);
        }

        #region Constructors
        public TranslationViewModel()
        {
            Search = new RelayCommand(DoSearch);
        }
        public TranslationViewModel(TranslationModel translation)
        {
            InitializeViewModel(translation);
        }
        #endregion

        #region EventHandlers
        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (!string.IsNullOrEmpty(EntityId))
            {
                InitializeViewModel(EntityId);
            }
            if (Model != null)
            {
                InitializeViewModel(Model);
            }
        }
        #endregion
        protected void InitializeViewModel(string entryId)
        {

        }

        protected void InitializeViewModel(TranslationModel translation)
        {
            Content = translation.Content;
            LanguageCode = translation.Language.Code;
            Notes = translation.Notes;

            Search = new RelayCommand(DoSearch);
        }
    }
}
