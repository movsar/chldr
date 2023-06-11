using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Models
{
    public abstract class EntryModel : IEntry
    {
        public string EntryId { get; internal set; }
        public string UserId { get; set; }
        public string? ParentEntryId { get; set; }
        public int Rate { get; set; }
        public int Type { get; set; }
        public abstract string Content { get; set; }
        public SourceModel Source { get; set; }
        public string? SourceId => Source.SourceId;
        public List<TranslationModel> Translations { get; set; } = new List<TranslationModel>();
        public abstract DateTimeOffset CreatedAt { get; set; }
        public abstract DateTimeOffset UpdatedAt { get; set; }
        protected void SetEntryFields(IEntryEntity entry, ISourceEntity source, IEnumerable<KeyValuePair<ILanguageEntity, ITranslationEntity>> translationEntityInfos)
        {
            EntryId = entry.EntryId;
            UserId = entry.UserId;
            ParentEntryId = entry.ParentEntryId;
            Rate = entry.Rate;
            Type = entry.Type;
            Source = SourceModel.FromEntity(source);
            CreatedAt = entry.CreatedAt;
            UpdatedAt = entry.UpdatedAt;

            foreach (var translationEntityToLanguage in translationEntityInfos)
            {
                Translations.Add(TranslationModel.FromEntity(translationEntityToLanguage.Value, translationEntityToLanguage.Key));
            }
        }
    }
}
