namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface IEntry
    {
        string EntryId { get; }
        string? ParentEntryId { get; }
        string? SourceId { get; }
        int Rate { get; }
        DateTimeOffset CreatedAt { get; }
        DateTimeOffset UpdatedAt { get; }
    }
}
