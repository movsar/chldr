using chldr_data.Entities;
using chldr_data.Interfaces;
using Newtonsoft.Json;

namespace chldr_data.Models
{
    public class TranslationModel : ITranslation
    {
        public string? TranslationId { get; internal set; }
        public string EntryId { get; set; }
        public string Content { get; }
        public LanguageModel Language { get; }
        public int Rate { get; set; } = 1;
        public string UserId { get; set; }
        public string LanguageId { get; set; }
        public string? Notes { get; }

        public TranslationModel(SqlTranslation translation)
        {
            TranslationId = translation.TranslationId;
            EntryId = translation.EntryId;
            Content = translation.Content;
            UserId = translation.UserId;
            Notes = translation.Notes;
            Rate = translation.Rate;
            Language = new LanguageModel(translation.Language);
            LanguageId = translation.Language.LanguageId;
        }

        public TranslationModel(RealmTranslation translation)
        {
            TranslationId = translation.TranslationId;
            EntryId = translation.EntryId;
            Content = translation.Content;
            UserId = translation.UserId;
            Notes = translation.Notes;
            Rate = translation.Rate;
            Language = new LanguageModel(translation.Language);
            LanguageId = translation.Language.LanguageId;
        }
    }
}
