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
    public class SqlSoundsRepository : SqlRepository<SqlSound, SoundModel, SoundDto>, ISoundsRepository
    {
        public SqlSoundsRepository(SqlContext context, FileService fileService, string userId) : base(context, fileService, userId) { }
        protected override RecordType RecordType => RecordType.Sound;
        protected override SoundModel FromEntityShortcut(SqlSound entity)
        {
            return SoundModel.FromEntity(entity);
        }
        public override async Task<List<ChangeSetModel>> Add(SoundDto soundDto)
        {
            if (string.IsNullOrEmpty(soundDto.RecordingB64))
            {
                throw new NullReferenceException("Sound data is empty");
            }

            _fileService.AddEntrySound(soundDto.FileName, soundDto.RecordingB64!);

            var sound = SqlSound.FromDto(soundDto, _dbContext);
            _dbContext.Sounds.Add(sound);

            var changeSet = CreateChangeSetEntity(Operation.Insert, soundDto.SoundId);
            _dbContext.ChangeSets.Add(changeSet);
            _dbContext.SaveChanges();

            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<List<ChangeSetModel>> Update(SoundDto dto)
        {
            // Find out what has been changed
            var existing = await Get(dto.SoundId);
            var existingDto = SoundDto.FromModel(existing);

            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                return new List<ChangeSetModel>();
            }
            ApplyChanges<SqlSound>(dto.SoundId, changes);

            var changeSet = CreateChangeSetEntity(Operation.Update, dto.SoundId);
            _dbContext.ChangeSets.Add(changeSet);
            _dbContext.SaveChanges();

            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<List<ChangeSetModel>> Remove(string entityId)
        {
            var sound = await _dbContext.Sounds.FindAsync(entityId);
            if (sound == null)
            {
                return new List<ChangeSetModel>();
            }

            _fileService.DeleteEntrySound(sound.FileName);

            return await base.Remove(entityId);
        }
    }
}
