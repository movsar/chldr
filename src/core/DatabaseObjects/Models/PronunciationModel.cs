using core.DatabaseObjects.Dtos;
using core.DatabaseObjects.Interfaces;

namespace core.DatabaseObjects.Models
{
    public class PronunciationModel : IPronunciation
    {
        public string FileName { get; set; }
        public string SoundId { get; set; }
        public string EntryId { get; set; }
        public string UserId { get; set; }
        public int Rate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public static PronunciationModel FromEntity(ISoundEntity entity)
        {
            return new PronunciationModel()
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
