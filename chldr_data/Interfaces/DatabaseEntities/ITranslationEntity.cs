using chldr_data.Entities;

namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface ITranslationEntity : ITranslation
    {
        string RawContents { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
