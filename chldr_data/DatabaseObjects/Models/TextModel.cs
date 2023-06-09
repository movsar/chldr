using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Models
{
    public class TextModel : EntryModel, IText
    {
        private TextModel() { }
        public string TextId { get; set; }
        public override string Content { get; set; }
        public override DateTimeOffset CreatedAt { get; set; }
        public override DateTimeOffset UpdatedAt { get; set; }

        public static TextModel FromEntity(IEntryEntity entry, ITextEntity text, ISourceEntity source, IEnumerable<KeyValuePair<ILanguageEntity, ITranslationEntity>> translationEntityInfos)
        {
            var wordModel = new TextModel()
            {
                EntryId = entry.EntryId,
                UserId = entry.UserId,
                ParentEntryId = entry.ParentEntryId,
                Rate = entry.Rate,
                Type = entry.Type,
                Source = SourceModel.FromEntity(source),
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt,

                TextId = text.TextId,
                Content = text.Content,
            };

            foreach (var translationEntityToLanguage in translationEntityInfos)
            {
                wordModel.Translations.Add(TranslationModel.FromEntity(translationEntityToLanguage.Value, translationEntityToLanguage.Key));
            }

            return wordModel;
        }
    }
}
