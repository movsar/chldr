using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Dtos
{
    internal class SoundDto : ISound
    {
        public string FileName { get;set; }
        public string SoundId { get;set; }
        public DateTimeOffset CreatedAt { get;set; }
        public DateTimeOffset UpdatedAt { get;set; }
    }
}
