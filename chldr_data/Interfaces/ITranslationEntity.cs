using chldr_data.Entities;
using chldr_data.Interfaces.DatabaseEntities;

namespace chldr_data.Interfaces
{
    public interface ITranslationEntity : ITranslation
    {
        string RawContents { get; set; }
        IEntryEntity Entry { get; set; }
        ILanguageEntity Language { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
