using chldr_data.Enums;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.Models
{
    public class EntryModelFactory
    {
        public static EntryModel CreateEntryModel(
            IEntryEntity entry, 
            IWordEntity word,
            IPhraseEntity phrase,
            ITextEntity text,
            ISourceEntity source,
            IEnumerable<KeyValuePair<ILanguageEntity, ITranslationEntity>> translations)
        {
            switch ((EntryType)entry?.Type)
            {
                case EntryType.Word:
                    return WordModel.FromEntity(entry, word, source, translations);
                case EntryType.Phrase:
                    return PhraseModel.FromEntity(entry, phrase, source, translations);
                case EntryType.Text:
                    return TextModel.FromEntity(entry, text, source, translations);
                default:
                    return null;
            }
        }
    }
}
