using Data.Models;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_blazor.ViewModels
{
    public class WordViewModel : ComponentBase
    {
        [Parameter]
        public string EntityId { get; set; }
        public int PartOfSpeech { get; set; }
        public int GrammaticalClass { get; set; }
        public string Content { get; set; }
        public string Notes { get; set; }
        public string RawForms { get; }
        public string RawVerbTenses { get; }
        public string RawNounDeclensions { get; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (string.IsNullOrEmpty(EntityId))
            {
                return;
            }

            InitializeViewModel(EntityId);
        }

        private void InitializeViewModel(string entityId)
        {
            InitializeViewModel(App.ContentStore.GetEntryById(ObjectId.Parse(entityId)));
        }

        private void InitializeViewModel(EntryModel entry)
        {
            var word = entry as WordModel;
            EntityId = word.EntityId.ToString();
            GrammaticalClass = word.GrammaticalClass;
            Content = word.Content;
            Notes = word.Notes;
            PartOfSpeech = word.PartOfSpeech;
        }

        public WordViewModel()
        {
        }
    }
}
