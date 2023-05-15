using chldr_data.Entities;
using chldr_data.Interfaces;

namespace chldr_data.Models
{
    public class TranslationModel : ITranslation
    {
        public string EntryId { get; set; }
        public string Content { get; }
        public string Notes { get; }
        public LanguageModel Language { get; }
        public int Rate { get; set; }
        public string? TranslationId { get; internal set; }
        public string UserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string LanguageId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TranslationModel(RealmTranslation translation)
        {
            TranslationId = translation.TranslationId;
            EntryId = translation.Entry.EntryId;
            Content = translation.Content;
            Notes = translation.Notes;
            Rate = translation.Rate;
            Language = new LanguageModel(translation.Language);
        }
    }
}
