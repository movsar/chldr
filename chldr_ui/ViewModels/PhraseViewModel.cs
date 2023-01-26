using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_shared.Stores;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;

namespace chldr_ui.ViewModels
{
    public class PhraseViewModel : EntryViewModelBase
    {
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Phrase = Entry as PhraseModel;
        }
        public PhraseModel? Phrase { get; set; }
        public string? Header => Phrase?.Content;
        public string? Subheader => Phrase?.Notes;
    }
}
