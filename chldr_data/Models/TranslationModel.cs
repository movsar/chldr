using chldr_data.Entities;

namespace chldr_data.Models
{
    public class TranslationModel : PersistentModelBase
    {
        public string EntryId { get; }
        public string Content { get; }
        public string Notes { get; }
        public LanguageModel Language { get; }
        public int Rate { get; set; }
        public TranslationModel(SqlTranslation translation) : base(translation)
        {
            EntryId = translation.Entry.EntryId;
            Content = translation.Content;
            Notes = translation.Notes;
            Rate = translation.Rate;
            Language = new LanguageModel(translation.Language);
        }
    }
}
