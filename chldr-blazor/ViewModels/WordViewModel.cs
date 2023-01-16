﻿using Data.Models;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_blazor.ViewModels
{
    public class WordViewModel : EntryViewModelBase
    {
        #region Properties
        public int PartOfSpeech { get; set; }
        public int GrammaticalClass { get; set; }
        public string Content { get; set; }
        public string Notes { get; set; }
        public string RawForms { get; }
        public string RawVerbTenses { get; }
        public string RawNounDeclensions { get; }
        #endregion
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

        public WordViewModel(WordModel word)
        {
            InitializeViewModel(word);
        }
        public WordViewModel() { }
        protected override void InitializeViewModel(EntryModel entry)
        {
            base.InitializeViewModel(entry);

            var word = entry as WordModel;

            EntityId = word.EntityId.ToString();
            GrammaticalClass = word.GrammaticalClass;
            Content = word.Content;
            Notes = word.Notes;
            PartOfSpeech = word.PartOfSpeech;

            var subheader = BuildWordInfoSubheader(word.Content, word.GrammaticalClass, word.RawForms, word.RawNounDeclensions, word.RawVerbTenses);
            Header = word.Content;
            Subheader = subheader;

            //case TextModel:
            //    var text = entry as TextModel;
            //    Header = text.Content;
            //    Type = EntryType.Text;

            //    break;
        }

        protected override void InitializeViewModel(string entryId)
        {
            InitializeViewModel(App.ContentStore.GetWordById(ObjectId.Parse(entryId)));
        }
    }
}