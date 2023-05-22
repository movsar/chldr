using chldr_data.DatabaseObjects.DatabaseEntities;
using Newtonsoft.Json;

namespace chldr_data.DatabaseObjects.Models
{
    public class TranslationModel : ITranslation
    {
        private TranslationModel() { }
        public string? TranslationId { get; internal set; }
        public string EntryId { get; set; }
        public string Content { get; set; }
        public LanguageModel Language { get; set; }
        public int Rate { get; set; } = 1;
        public string UserId { get; set; }
        public string LanguageId { get; set; }
        public string? Notes { get; set; }
        public static TranslationModel FromEntity(ITranslationEntity translation, ILanguageEntity language)
        {
            return new TranslationModel()
            {
                TranslationId = translation.TranslationId,
                EntryId = translation.EntryId,
                Content = translation.Content,
                UserId = translation.UserId,
                Notes = translation.Notes,
                Rate = translation.Rate,
                Language = LanguageModel.FromEntity(language),
                LanguageId = language.LanguageId,
            };
        }
    }
}
