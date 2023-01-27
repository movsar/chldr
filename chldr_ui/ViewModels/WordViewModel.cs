using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_shared.Stores;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.ViewModels
{
    public class WordViewModel : EntryViewModelBase
    {
        [Inject] NavigationManager NavigationManager { get; set; }
        public WordModel? Word { get; set; }
     
        public string? Header => Word?.Content;
        public string? Subheader => CreateSubheader();
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Word == null && Entry != null)
            {
                Word = Entry as WordModel;

                Translations = Entry.Translations;
                Source = ParseSource(Entry.Source.Name);
            }
        }
        private string CreateSubheader()
        {
            if (Word.RawNounDeclensions == chldr_data.Entities.Word.EmptyRawWordDeclensionsValue && Word.RawVerbTenses == chldr_data.Entities.Word.EmptyRawWordTensesValue)
            {
                return null;
            }
            var allForms = chldr_data.Entities.Word.GetAllUniqueWordForms(Word.Content, Word.RawForms, Word.RawNounDeclensions, Word.RawVerbTenses, true);
            string part1 = String.Join(", ", allForms);
            if (part1.Length == 0)
            {
                return null;
            }

            string part2 = $" {chldr_data.Entities.Word.ParseGrammaticalClass(Word.GrammaticalClass)} ";

            return $"[{part1}{part2}]";
        }
    }
}
