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

        public static PhraseModel FromEntity(IEntryEntity entryEntity, IPhraseEntity phraseEntity, ISourceEntity sourceEntity, IEnumerable<KeyValuePair<ILanguageEntity, ITranslationEntity>> translationEntityInfos)
        {
            var phraseModel = new PhraseModel()
            {
                EntryId = entryEntity.EntryId,
                Rate = entryEntity.Rate,
                Type = entryEntity.Type,
                Source = SourceModel.FromEntity(sourceEntity),
                CreatedAt = entryEntity.CreatedAt,
                UpdatedAt = entryEntity.UpdatedAt,

                PhraseId = phraseEntity.PhraseId,
                Content = phraseEntity.Content,
                Notes = phraseEntity.Notes,
            };

            foreach (var translationEntityToLanguage in translationEntityInfos)
            {
                phraseModel.Translations.Add(TranslationModel.FromEntity(translationEntityToLanguage.Value, translationEntityToLanguage.Key));
            }

            return phraseModel;
        }

    }
}
