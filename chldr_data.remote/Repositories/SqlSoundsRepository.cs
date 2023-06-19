using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_utils.Services;

namespace chldr_data.remote.Repositories
{
    internal class SqlSoundsRepository : SqlRepository<SqlSound, SoundModel, SoundDto>, ISoundsRepository
    {
        public SqlSoundsRepository(SqlContext context, string userId) : base(context, userId) { }

        protected override RecordType RecordType => RecordType.Sound;
        public override void Remove(string entityId)
        {
            var sound = Get(entityId);

            base.Remove(entityId);
          
            var filePath = Path.Combine(FileService.EntrySoundsDirectory, sound.FileName);
            File.Delete(filePath);
        }
        public override void Add(SoundDto soundDto)
        {
            if (string.IsNullOrEmpty(soundDto.RecordingB64))
            {
                throw new NullReferenceException("Sound data is empty");
            }

            var data = Convert.FromBase64String(soundDto.RecordingB64);
            var filePath = Path.Combine(FileService.EntrySoundsDirectory, soundDto.FileName);
            File.WriteAllBytes(filePath, data);

            var sound = SqlSound.FromDto(soundDto, _dbContext);
            _dbContext.Sounds.Add(sound);
            _dbContext.SaveChanges();

            InsertChangeSet(Operation.Insert, _userId, sound.SoundId);
        }

        public override void Update(SoundDto dto)
        {
            // Find out what has been changed
            var existing = Get(dto.SoundId);
            var existingDto = SoundDto.FromModel(existing);

            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                return;
            }
            ApplyChanges<SqlSound>(dto.SoundId, changes);

            InsertChangeSet(Operation.Update, _userId, dto.SoundId, changes);
        }

        protected override SoundModel FromEntityShortcut(SqlSound entity)
        {
            return SoundModel.FromEntity(entity);
        }
    }
}
