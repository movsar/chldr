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
    public partial class EntryViewModel
    {
        [Inject] private NavigationManager NavigationManager { get; set; }

        public ObjectId EntryId { get; }
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

        public WordModel Word { get; }
        public PhraseModel Phrase { get; }
        public TextModel Text { get; }

        public ObjectId TargetObjectId { get; set; }
        public EntryViewModel() { }

        public EntryViewModel(EntryModel entry)
        {
            _navigationManager = NavigationManager;
            EntryId = entry.EntryId;
            Type = entry.Type;
            Source = ParseSource(entry.Source.Name);
            Translations.AddRange(entry.Translations.Select(t => new TranslationViewModel(t)));
            Notes = entry.Notes;
            TargetObjectId = entry.Target.TargetId;

            switch (entry.Type)
            {
                case EntryType.Word:
                    Word = entry.Target as WordModel;

                    var subheader = BuildWordInfoSubheader(Word.Content, Word.GrammaticalClass, Word.RawForms, Word.RawNounDeclensions, Word.RawVerbTenses);
                    Header = Word.Content;
                    Subheader = subheader;

                    break;
                case EntryType.Phrase:
                    Phrase = entry.Target as PhraseModel;
                    Header = Phrase.Content;
                    Subheader = Phrase.Notes;

                    break;
                case EntryType.Text:
                    Text = entry.Target as TextModel;
                    Header = Text.Content;

                    break;
            }
        }
        private static string ParseSource(string sourceName)
        {
            string? sourceTitle = null;
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
