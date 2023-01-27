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
        public PhraseModel? Phrase { get; set; }
        public string? Header => Phrase?.Content;
        public string? Subheader => Phrase?.Notes;
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Phrase == null && Entry != null)
            {
                Phrase = Entry as PhraseModel;

                Translations = Entry.Translations;
                Source = ParseSource(Entry.Source.Name);
            }
        }
    }
}
