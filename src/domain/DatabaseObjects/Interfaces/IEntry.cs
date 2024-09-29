namespace domain.DatabaseObjects.Interfaces
{
    public interface IEntry
    {
        string EntryId { get; }
        string? UserId { get; }
        string? ParentEntryId { get; }
        string Content { get; }
        int Rate { get; }
        DateTimeOffset CreatedAt { get; }
        DateTimeOffset UpdatedAt { get; }
    }
}
