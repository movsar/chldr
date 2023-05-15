using chldr_data.Entities;
using chldr_data.Interfaces;

namespace chldr_data.Models
{
    public class TranslationModel : ITranslation
    {
        public string EntryId { get; set; }
        public string Content { get; }
        public LanguageModel Language { get; }
        public int Rate { get; set; } = 1;
        public string UserId { get; set; }
        public string LanguageId { get; set; }
        
        public string? TranslationId { get; internal set; }
        public string? Notes { get; }

        public TranslationModel(RealmTranslation translation)
        {
            TranslationId = translation.TranslationId;
            EntryId = translation.Entry.EntryId;
            Content = translation.Content;
            UserId = translation.User.UserId;
            Notes = translation.Notes;
            Rate = translation.Rate;
            Language = new LanguageModel(translation.Language);
            LanguageId = translation.Language.LanguageId;
        }
    }
}
