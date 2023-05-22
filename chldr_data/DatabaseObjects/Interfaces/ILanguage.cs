namespace chldr_data.DatabaseObjects.DatabaseEntities
{
    public interface ILanguage : IEntity
    {
        string Code { get; }
        string? LanguageId { get; }
        string Name { get; set; }
    }
}