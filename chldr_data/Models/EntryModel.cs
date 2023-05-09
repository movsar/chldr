using chldr_data.Entities;
namespace chldr_data.Models
{
    public abstract class EntryModel : PersistentModelBase
    {
        public abstract string Content { get; }
        public List<TranslationModel> Translations { get; } = new List<TranslationModel>();
        public SourceModel Source { get; }
        public int Rate { get; }
        public int Type { get; }
        public string? EntryId { get; internal set; }

        public EntryModel(RealmEntry entry)
        {
            EntryId = entry.EntryId;
            Rate = entry.Rate;
            Type = entry.Type;
            Source = new SourceModel(entry.Source);

            foreach (var translationEntity in entry.Translations)
            {
                Translations.Add(new TranslationModel(translationEntity));
            }
        }
    }
}
