using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;

namespace chldr_data.DatabaseObjects.Dtos
{
    public class SoundDto : ISound
    {
        public string SoundId { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; set; } = Guid.NewGuid().ToString();
        public string EntryId { get; set; }
        public string UserId { get; set; }
        public virtual string RecordingB64 { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public static SoundDto FromModel(SoundModel sound)
        {
            if (sound == null || string.IsNullOrEmpty(sound.FileName))
            {
                throw new Exception("Something went wrong");
            }

            return new SoundDto()
            {
                EntryId = sound.EntryId,
                FileName = sound.FileName,
                CreatedAt = sound.CreatedAt,
                SoundId = sound.SoundId,
                UserId = sound.UserId,
            };
        }
    }
}
