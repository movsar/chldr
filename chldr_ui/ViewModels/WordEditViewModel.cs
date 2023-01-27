using chldr_data.Models;
using chldr_shared.Stores;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.ViewModels
{
    public class WordEditViewModel : EntryEditViewModelBase<WordModel, WordValidator>
    {
        #region Properties
        [Parameter]
        public string? WordId { get; set; }
        public int PartOfSpeech { get; set; }
        public int GrammaticalClass { get; set; }
        public string Content { get; set; }
        public string Notes { get; set; }
        public string RawForms { get; }
        public string RawVerbTenses { get; }
        public string RawNounDeclensions { get; }
        #endregion

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (string.IsNullOrEmpty(WordId))
            {
                return;
            }

            InitializeViewModel(ContentStore.GetWordById(new ObjectId(WordId)));
        }

        protected override void InitializeViewModel(EntryModel entry)
        {
            base.InitializeViewModel(entry);

            var word = entry as WordModel;
            if (word == null)
            {
                throw new Exception("Error:Word_shouldn't_be_null");
            }

            EntryId = word.EntryId;
            PartOfSpeech = word.PartOfSpeech;
            GrammaticalClass = word.GrammaticalClass;
            Content = word.Content;
            Notes = word.Notes;
        }
    }
}
