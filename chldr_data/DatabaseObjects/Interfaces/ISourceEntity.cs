namespace chldr_data.DatabaseObjects.DatabaseEntities
{
    public interface ISourceEntity : ISource, IEntity
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
