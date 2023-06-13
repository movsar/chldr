namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface IEntry
    {
        string EntryId { get; }
        string UserId { get; }
        string SourceId { get; }
        string? ParentEntryId { get; }
        string Content { get; }
        int Rate { get; }
        DateTimeOffset CreatedAt { get; }
        DateTimeOffset UpdatedAt { get; }
    }
}
