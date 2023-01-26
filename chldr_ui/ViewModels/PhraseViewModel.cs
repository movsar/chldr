using chldr_data.Enums;
using chldr_data.Models;
using chldr_shared.Stores;
using MongoDB.Bson;

namespace chldr_ui.ViewModels
{
    public class PhraseViewModel : EntryViewModelBase
    {
        public string? Content { get; set; }
        public string? Notes { get; set; }
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
