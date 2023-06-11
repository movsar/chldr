using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums.WordDetails;

namespace chldr_data.DatabaseObjects.Models
{
    public class TextModel : EntryModel, IText
    {
        private TextModel() { }
        public string TextId { get; set; }
        public override string Content { get; set; }
        public override DateTimeOffset CreatedAt { get; set; }
        public override DateTimeOffset UpdatedAt { get; set; }
        public static TextModel FromEntity(ITextEntity text, IEntryEntity entry, ISourceEntity source, IEnumerable<KeyValuePair<ILanguageEntity, ITranslationEntity>> translationEntityInfos)
        {
            var textModel = new TextModel()
            {
                TextId = text.TextId,
                Content = text.Content,
            };

            textModel.SetEntryFields(entry, source, translationEntityInfos);
            return textModel;
        }
    }
}
