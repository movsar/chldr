using chldr_data.Enums;
using chldr_data.Models.Words;

namespace chldr_data.Models
{
    internal class EntryModelFactory
    {
        public static EntryModel CreateEntryModel(Entities.RealmEntry entryEntity)
        {
            switch ((EntryType)entryEntity?.Type)
            {
                case EntryType.Word:
                    return WordModel.FromEntity(entryEntity.Word);
                case EntryType.Phrase:
                    return PhraseModel.FromEntity(entryEntity.Phrase);
                case EntryType.Text:
                    return TextModel.FromEntity(entryEntity.Text);
                default:
                    return null;
            }
        }
    }
}
