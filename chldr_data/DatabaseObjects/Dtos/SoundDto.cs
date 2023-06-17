using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class SoundDto : ISound
    {
        public string EntryId { get; set; }
        public string SoundId { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
