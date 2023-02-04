using chldr_data.Entities;
namespace chldr_data.Models
{
    //public class EntryComparer : IComparer<EntryModel>
    //{
    //    public int Compare(EntryModel? x, EntryModel? y)
    //    {
    //        if (x.Content == y.Content)
    //        {
    //            return 0;
    //        }

    //        if (x.Content.s)
    //    }
    //}
    public abstract class EntryModel : ModelBase
    {
        public abstract string Content { get; }
        public List<TranslationModel> Translations { get; } = new List<TranslationModel>();
        public SourceModel Source { get; }
        public int Rate { get; }
        public int Type { get; }
        public EntryModel(Entry entry) : base(entry)
        {
            Rate = entry.Rate;
            Type = entry.Type;

            Source = new SourceModel(entry.Source);

            foreach (var translationEntity in entry.Translations)
            {
                Translations.Add(new TranslationModel(translationEntity));
            }
        }
    }
}
