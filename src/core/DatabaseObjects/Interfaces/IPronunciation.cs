namespace core.DatabaseObjects.Interfaces
{
    public interface IPronunciation
    {
        string SoundId { get; set; }
        string EntryId { get; }
        string UserId { get; }
        string FileName { get; }
        int Rate { get; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
