using chldr_data.Dto;

namespace chldr_data.Interfaces.DatabaseEntities
{
    public interface ISourceEntity : ISource, IEntity
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
