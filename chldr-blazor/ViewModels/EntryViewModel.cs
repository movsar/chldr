using CommunityToolkit.Mvvm.ComponentModel;
using Data.Entities;
using Data.Enums;
using Data.Models;
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
        public ICommand TranslationSelectedCommand { get; set; } = new Command((e) =>
        {
            var translationViewModel = ((dynamic)e).SelectedItem;
            translationViewModel.CurrentTranslationSelected();
        });

        public string Source { get; }
        public string Header { get; set; }
        public string Subheader { get; set; }
        public int Type { get; set; }
        public List<TranslationViewModel> Translations { get; } = new();
        public EntryViewModel() { }
        public EntryViewModel(EntryModel entry)
        {
            Type = entry.Type;
            Source = ParseSource(entry.Source.Name);
            Translations.AddRange(entry.Translations.Select(t => new TranslationViewModel(t)));
            switch (entry.Type)
            {
                case EntryType.Word:
                    var word = entry.Target as WordModel;

                    var subheader = BuildWordInfoSubheader(word.Content, word.GrammaticalClass, word.RawForms, word.RawNounDeclensions, word.RawVerbTenses);
                    Header = word.Content;
                    Subheader = subheader;

                    break;
                case EntryType.Phrase:
                    var phrase = entry.Target as PhraseModel;
                    Header = phrase.Content;
                    Subheader = phrase.Notes;

                    break;
                case EntryType.Text:
                    var text = entry.Target as TextModel;
                    Header = text.Content;

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
        private static string BuildWordInfoSubheader(string content, string rawWordGrammaticalClass, string rawForms, string rawWordDeclensions, string rawWordTenses)
        {
            if (rawWordDeclensions == WordEntity.EmptyRawWordDeclensionsValue && rawWordTenses == WordEntity.EmptyRawWordTensesValue)
            {
                return null;
            }
            var allForms = WordEntity.GetAllUniqueWordForms(content, rawForms, rawWordDeclensions, rawWordTenses, true);
            string part1 = String.Join(", ", allForms);
            if (part1.Length == 0)
            {
                return null;
            }

            string part2 = $" {WordEntity.ParseGrammaticalClass(rawWordGrammaticalClass)} ";

            return $"[{part1}{part2}]";
        }
    }
}
