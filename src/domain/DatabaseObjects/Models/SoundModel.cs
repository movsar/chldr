using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Interfaces;

namespace domain.DatabaseObjects.Models
{
    public class SoundModel : ISound
    {
        public string FileName { get; set; }
        public string SoundId { get; set; }
        public string EntryId { get; set; }
        public string UserId { get; set; }
        public int Rate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public static SoundModel FromEntity(ISoundEntity entity)
        {
            return new SoundModel()
            {
                SoundId = entity.SoundId,
                UserId = entity.UserId,
                EntryId = entity.EntryId,
                FileName = entity.FileName,
                Rate = entity.Rate,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
            };
        }
    }
}
