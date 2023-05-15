using chldr_data.Interfaces.DatabaseEntities;

namespace chldr_data.Interfaces
{
    public interface ITranslation : IEntity
    {
        string TranslationId { get;  }
        string EntryId { get; }
        string UserId { get; }
        string LanguageId { get; }
        string Content { get; }
        int Rate { get; }
        string? Notes { get; }
    }
}
