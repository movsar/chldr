using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Models
{
    public class EntryModel : IEntry
    {
        public string EntryId { get; internal set; }
        public string UserId { get; set; }
        public string? ParentEntryId { get; set; }
        public int Rate { get; set; }
        public int Type { get; set; }
        public int Subtype { get; set; }
        public string Content { get; set; }
        public string RawContents { get; set; }
        public string Details { get; set; }
        public SourceModel Source { get; set; }
        public string? SourceId => Source.SourceId;
        public List<TranslationModel> Translations { get; set; } = new List<TranslationModel>();
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public static EntryModel FromEntity(IEntryEntity entry, ISourceEntity source, IEnumerable<KeyValuePair<ILanguageEntity, ITranslationEntity>> translationEntityInfos)
        {
            var entryModel = new EntryModel()
            {
                EntryId = entry.EntryId,
                UserId = entry.UserId,
                ParentEntryId = entry.ParentEntryId,
                Rate = entry.Rate,
                Type = entry.Type,
                Subtype = entry.Subtype,
                Source = SourceModel.FromEntity(source),
                Content = entry.Content,
                RawContents = entry.RawContents,
                Details = entry.Details,
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt
            };

            foreach (var translationEntityToLanguage in translationEntityInfos)
            {
                entryModel.Translations.Add(TranslationModel.FromEntity(translationEntityToLanguage.Value, translationEntityToLanguage.Key));
            }

            return entryModel;
        }
    }
}
