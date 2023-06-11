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
                    return WordModel.FromEntity(word, entry, source, translations);
                case EntryType.Phrase:
                    return PhraseModel.FromEntity(phrase, entry, source, translations);
                case EntryType.Text:
                    return TextModel.FromEntity(text, entry, source, translations);
                default:
                    return null;
            }
        }
    }
}
