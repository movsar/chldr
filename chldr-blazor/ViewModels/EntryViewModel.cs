using CommunityToolkit.Mvvm.ComponentModel;
using Data.Entities;
using Data.Enums;
using Data.Models;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace chldr_blazor.ViewModels
{
    [ObservableObject]
    public partial class EntryViewModel : ComponentBase
    {
        [Parameter]
        public string EntryId { get; set; }
        public string Source { get; set; }
        public string Header { get; set; }
        public string Subheader { get; set; }
        public string Notes { get; set; }

        private NavigationManager _navigationManager;

        public int Type { get; set; }
        public List<TranslationViewModel> Translations { get; } = new();

        public ICommand TranslationSelectedCommand { get; set; } = new Command((e) =>
            {
                var translationViewModel = ((dynamic)e).SelectedItem;
                translationViewModel.CurrentTranslationSelected();
            });

        #region Actions
        public void ListenToPronunciation() { }
        public void NewTranslation() { }
        public void AddToFavorites() { }
        public void Share() { }
        public void Flag() { }
        #endregion

        public EntryViewModel() { }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (string.IsNullOrEmpty(EntryId))
            {
                return;
            }

            InitializeViewModel(EntryId);
        }

        private void InitializeViewModel(string entryId)
        {
            InitializeViewModel(App.ContentStore.GetEntryById(ObjectId.Parse(entryId)));
        }
        private void InitializeViewModel(EntryModel entry)
        {
            EntryId = entry.EntityId.ToString();
            Source = ParseSource(entry.Source.Name);
            Translations.AddRange(entry.Translations.Select(t => new TranslationViewModel(t)));
            Notes = entry.Notes;

            switch (entry)
            {
                case WordModel:
                    var word = entry as WordModel;

                    var subheader = BuildWordInfoSubheader(word.Content, word.GrammaticalClass, word.RawForms, word.RawNounDeclensions, word.RawVerbTenses);
                    Header = word.Content;
                    Subheader = subheader;
                    Type = EntryType.Word;

                    break;
                case PhraseModel:
                    var phrase = entry as PhraseModel;
                    Header = phrase.Content;
                    Subheader = phrase.Notes;
                    Type = EntryType.Phrase;

                    break;
                case TextModel:
                    var text = entry as TextModel;
                    Header = text.Content;
                    Type = EntryType.Text;

                    break;
            }
        }

        public EntryViewModel(EntryModel entry)
        {
            InitializeViewModel(entry);
        }
        private static string ParseSource(string sourceName)
        {
            string sourceTitle = null;
            switch (sourceName)
            {
                case "Maciev":
                    sourceTitle = "Чеченско - русский словарь, А.Г.Мациева";
                    break;
                case "Karasaev":
                    sourceTitle = "Русско - чеченский словарь, Карасаев А.Т., Мациев А.Г.";
                    break;
                case "User":
                    sourceTitle = "Добавлено пользователем";
                    break;
                case "Malaev":
                    sourceTitle = "Чеченско - русский словарь, Д.Б. Малаева";
                    break;
                case "Anatslovar":
                    sourceTitle = "Чеченско-русский, русско-чеченский словарь анатомии человека, Р.У. Берсанова";
                    break;
                case "ikhasakhanov":
                    sourceTitle = "Ислам Хасаханов";
                    break;
            }
            return sourceTitle;
        }
        private static string BuildWordInfoSubheader(string content, int grammaticalClass, string rawForms, string rawWordDeclensions, string rawWordTenses)
        {
            if (rawWordDeclensions == Data.Entities.Word.EmptyRawWordDeclensionsValue && rawWordTenses == Data.Entities.Word.EmptyRawWordTensesValue)
            {
                return null;
            }
            var allForms = Data.Entities.Word.GetAllUniqueWordForms(content, rawForms, rawWordDeclensions, rawWordTenses, true);
            string part1 = String.Join(", ", allForms);
            if (part1.Length == 0)
            {
                return null;
            }

            string part2 = $" {Data.Entities.Word.ParseGrammaticalClass(grammaticalClass)} ";

            return $"[{part1}{part2}]";
        }
    }
}
