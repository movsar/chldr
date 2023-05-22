using chldr_data.Entities;
using System.Threading.Tasks;

namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface ILanguageEntity : ILanguage
    {
        string? UserId { get; set; }
        IUserEntity? User { get; set; }
        ICollection<ITranslationEntity> Translations { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
