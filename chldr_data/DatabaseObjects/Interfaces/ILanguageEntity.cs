namespace chldr_data.DatabaseObjects.DatabaseEntities
{
    public interface ILanguageEntity : ILanguage
    {
        string? UserId { get; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
