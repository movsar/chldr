using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class SoundDto : ISound
    {
        public string SoundId { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; } = Guid.NewGuid().ToString();
        public string EntryId { get; set; }
        public string UserId { get; set; }
        public virtual string RecordingB64 { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
