namespace chldr_data.DatabaseObjects.DatabaseEntities
{
    public interface IEntry
    {
        string EntryId { get; }
        string? SourceId { get; }
        int Rate { get; }
        DateTimeOffset CreatedAt { get; }
        DateTimeOffset UpdatedAt { get; }
    }
}
