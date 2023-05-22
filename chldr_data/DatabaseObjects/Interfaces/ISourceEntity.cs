namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface ISourceEntity : ISource, IEntity
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
