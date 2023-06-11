using chldr_data.DatabaseObjects.Interfaces;
using System.Collections.Generic;

namespace chldr_data.DatabaseObjects.Models
{
    public class PhraseModel : EntryModel, IPhrase
    {
        private PhraseModel() { }
        public string PhraseId { get; set; }
        public override string Content { get; set; }
        public override DateTimeOffset CreatedAt { get; set; }
        public override DateTimeOffset UpdatedAt { get; set; }
        public static PhraseModel FromEntity(IPhraseEntity phrase, IEntryEntity entry,  ISourceEntity source, IEnumerable<KeyValuePair<ILanguageEntity, ITranslationEntity>> translationEntityInfos)
        {
            var phraseModel = new PhraseModel()
            {
                PhraseId = phrase.PhraseId,
                Content = phrase.Content,
            };

            phraseModel.SetEntryFields(entry, source, translationEntityInfos);
            return phraseModel;
        }
    }
}
