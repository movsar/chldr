using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Models
{
    public class PhraseModel : EntryModel, IPhrase
    {
        private PhraseModel() { }   
        public string PhraseId { get; set; }
        public override string Content { get; set; }
        public string? Notes { get; set; }
        public override DateTimeOffset CreatedAt { get; set; }
        public override DateTimeOffset UpdatedAt { get; set; }

        public static PhraseModel FromEntity(IEntryEntity entry, IPhraseEntity phrase, ISourceEntity source, IEnumerable<KeyValuePair<ILanguageEntity, ITranslationEntity>> translationEntityInfos)
        {
            var phraseModel = new PhraseModel()
            {
                EntryId = entry.EntryId,
                UserId = entry.UserId,
                ParentEntryId = entry.ParentEntryId,
                Rate = entry.Rate,
                Type = entry.Type,
                Source = SourceModel.FromEntity(source),
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt,

                PhraseId = phrase.PhraseId,
                Content = phrase.Content,
            };

            foreach (var translationEntityToLanguage in translationEntityInfos)
            {
                phraseModel.Translations.Add(TranslationModel.FromEntity(translationEntityToLanguage.Value, translationEntityToLanguage.Key));
            }

            return phraseModel;
        }

    }
}
