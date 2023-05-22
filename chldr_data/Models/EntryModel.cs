using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_data.Interfaces.DatabaseEntities;

namespace chldr_data.Models
{
    public abstract class EntryModel : IEntry
    {
        public List<TranslationModel> Translations { get; } = new List<TranslationModel>();
        public SourceModel Source { get; }
        public int Rate { get; }
        public int Type { get; }
        public string EntryId { get; internal set; }
        public abstract string Content { get; }
        public abstract DateTimeOffset CreatedAt { get; }
        public abstract DateTimeOffset UpdatedAt { get; }
        public string? SourceId => Source.SourceId;

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

        public EntryModel(SqlEntry entry)
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
