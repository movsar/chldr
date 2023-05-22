using chldr_data.DatabaseObjects.Interfaces;

namespace chldr_data.DatabaseObjects.Models
{
    public class SoundModel : ISound
    {
        public string FileName { get;set; }
        public string SoundId { get;set; }
        public DateTimeOffset CreatedAt { get;set; }
        public DateTimeOffset UpdatedAt { get;set; }
    }
}
