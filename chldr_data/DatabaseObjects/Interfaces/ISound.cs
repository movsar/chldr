namespace chldr_data.DatabaseObjects.Interfaces
{
    public interface ISound
    {
        string SoundId { get; set; }
        string EntryId { get; }
        string UserId { get; }
        string FileName { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
