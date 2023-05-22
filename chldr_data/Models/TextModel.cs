using chldr_data.Entities;
using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces.DatabaseEntities;
using chldr_data.Models.Words;

namespace chldr_data.Models
{
    public class TextModel : EntryModel, IText
    {
        public string TextId { get; set; }
        public override string Content { get; set; }
        public override DateTimeOffset CreatedAt { get; set; }
        public override DateTimeOffset UpdatedAt { get; set; }

        private static TextModel FromEntity(IEntryEntity entry, ITextEntity text, ISourceEntity source, IEnumerable<KeyValuePair<ILanguageEntity, ITranslationEntity>> translationEntityInfos)
        {
            var wordModel = new TextModel()
            {
                EntryId = entry.EntryId,
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

        public static TextModel FromEntity(SqlText entity)
        {
            return FromEntity(entity.Entry,
                entity.Entry.Text,
                entity.Entry.Source,
                entity.Entry.Translations.Select(
                    t => new KeyValuePair<ILanguageEntity, ITranslationEntity>(t.Language, t)
                ));
        }

        public static TextModel FromEntity(RealmText entity)
        {
            return FromEntity(entity.Entry,
                 entity.Entry.Text,
                 entity.Entry.Source,
                 entity.Entry.Translations.Select(
                     t => new KeyValuePair<ILanguageEntity, ITranslationEntity>(t.Language, t)
                 ));
        }
    }
}
