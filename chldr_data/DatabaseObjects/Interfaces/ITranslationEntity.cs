namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface ITranslationEntity : ITranslation
    {
        string RawContents { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
