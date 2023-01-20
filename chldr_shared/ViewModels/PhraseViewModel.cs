using chldr_dataaccess.Enums;
using chldr_dataaccess.Models;
using chldr_shared.Stores;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.ViewModels
{
    public class PhraseViewModel : EntryViewModelBase
    {
        [Inject] ContentStore ContentStore { get; set; }

        public string Content { get; set; }
        public string Notes { get; set; }
        public PhraseViewModel(PhraseModel phrase)
        {
            InitializeViewModel(phrase);
        }
        public PhraseViewModel() { }
        protected override void InitializeViewModel(EntryModel entry)
        {
            base.InitializeViewModel(entry);

            var phrase = entry as PhraseModel;

            EntityId = phrase.EntityId.ToString();
            Content = phrase.Content;
            Notes = phrase.Notes;

            Header = phrase.Content;
            Subheader = phrase.Notes;
            Type = EntryType.Phrase;
        }

        protected override void InitializeViewModel(string entryId)
        {
            InitializeViewModel(ContentStore.GetPhraseById(ObjectId.Parse(entryId)));
        }
    }
}
