using domain.DatabaseObjects.Interfaces;

namespace domain.DatabaseObjects.Models
{
    public class TranslationModel : ITranslation
    {
        public TranslationModel() { }
        public string TranslationId { get; set; }
        public string EntryId { get; set; }
        public string SourceId { get; set; }
        public string Content { get; set; }
        public int Rate { get; set; } = 1;
        public string UserId { get; set; }
        public string LanguageCode { get; set; }
        public string? Notes { get; set; }
        public virtual SourceModel Source { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public static TranslationModel FromEntity(ITranslationEntity entity)
        {
            return new TranslationModel()
            {
                TranslationId = entity.TranslationId,
                EntryId = entity.EntryId,
                Content = entity.Content,
                UserId = entity.UserId,
                SourceId = entity.SourceId,
                Notes = entity.Notes,
                Rate = entity.Rate,
                LanguageCode = entity.LanguageCode,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt                
            };
        }
        protected static string? ParseSource(string sourceName)
        {
            string? sourceTitle = null;
            switch (sourceName)
            {
                case "Maciev":
                    sourceTitle = "Чеченско - русский словарь, А.Г.Мациева";
                    break;
                case "Karasaev":
                    sourceTitle = "Русско - чеченский словарь, Карасаев А.Т., Мациев А.Г.";
                    break;
                case "User":
                    sourceTitle = "Добавлено пользователем";
                    break;
                case "Malaev":
                    sourceTitle = "Чеченско - русский словарь, Д.Б. Малаева";
                    break;
                case "Anatslovar":
                    sourceTitle = "Чеченско-русский, русско-чеченский словарь анатомии человека, Р.У. Берсанова";
                    break;
                case "ikhasakhanov":
                    sourceTitle = "Ислам Хасаханов";
                    break;
            }
            return sourceTitle;
        }
    }
}
