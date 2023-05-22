namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface ILanguageEntity : ILanguage
    {
        string? UserId { get; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
