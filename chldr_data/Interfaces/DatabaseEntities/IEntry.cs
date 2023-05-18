namespace chldr_data.Interfaces.DatabaseEntities
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
