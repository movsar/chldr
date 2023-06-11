using chldr_data.DatabaseObjects.Models;

namespace chldr_ui.ViewModels
{
    public class PhraseViewModel : EntryViewModelBase
    {
        public PhraseModel? Phrase { get; set; }
        public string? Header => Phrase?.Content;
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
