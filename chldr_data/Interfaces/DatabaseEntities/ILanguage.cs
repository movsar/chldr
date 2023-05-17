namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface ILanguage : IEntity
    {
        string Code { get; }
        string? LanguageId { get; set; }
        string Name { get; set; }
    }
}