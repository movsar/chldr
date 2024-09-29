namespace domain.DatabaseObjects.Interfaces
{
    public interface ITranslation : IEntity
    {
        string TranslationId { get; }
        string EntryId { get; }
        string SourceId { get; }
        string UserId { get; }
        string LanguageCode { get; }
        string Content { get; }
        int Rate { get; }
        string? Notes { get; }
        DateTimeOffset CreatedAt { get; }
        DateTimeOffset UpdatedAt { get; }
    }
}
