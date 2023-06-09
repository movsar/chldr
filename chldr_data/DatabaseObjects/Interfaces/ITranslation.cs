namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface ITranslation : IEntity
    {
        string TranslationId { get; }
        string EntryId { get; }
        string UserId { get; }
        string LanguageId { get; }
        string Content { get; }
        int Rate { get; }
        string? Notes { get; }
        DateTimeOffset CreatedAt { get; }
        DateTimeOffset UpdatedAt { get; }
    }
}
