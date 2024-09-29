using domain.DatabaseObjects.Interfaces;
using domain.DatabaseObjects.Models;

namespace domain.DatabaseObjects.Dtos
{
    public class SoundDto : ISound
    {
        public string SoundId { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; set; } = Guid.NewGuid().ToString();
        public string EntryId { get; set; }
        public string UserId { get; set; }
        public virtual string? RecordingB64 { get; set; }
        public int Rate { get; set; }
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
                SoundId = sound.SoundId,
                EntryId = sound.EntryId,
                UserId = sound.UserId,
                FileName = sound.FileName,
                Rate = sound.Rate,
                CreatedAt = sound.CreatedAt,
                UpdatedAt = sound.UpdatedAt,
            };
        }
    }
}
