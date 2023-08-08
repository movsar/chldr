using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.remote.Repositories
{
    public class SqlSoundsRepository : SqlRepository<SqlSound, PronunciationModel, PronunciationDto>, ISoundsRepository
    {
        public SqlSoundsRepository(DbContextOptions<SqlContext> dbConfig, FileService fileService, string userId) : base(dbConfig, fileService, userId) { }
        protected override RecordType RecordType => RecordType.Sound;
        protected override PronunciationModel FromEntity(SqlSound entity)
        {
            return PronunciationModel.FromEntity(entity);
        }
        public override async Task<List<PronunciationModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();
            var ids = await _dbContext.Set<SqlSound>().Select(e => e.SoundId).ToListAsync();
            var randomlySelectedIds = ids.OrderBy(x => randomizer.Next(1, Constants.EntriesApproximateCoount)).Take(limit).ToList();

            var entities = await _dbContext.Set<SqlSound>()
              .Where(e => randomlySelectedIds.Contains(e.SoundId))
              .AsNoTracking()
              .ToListAsync();

            var models = entities.Select(FromEntity).ToList();
            return models;
        }
        public override async Task<List<ChangeSetModel>> Add(PronunciationDto soundDto)
        {
            if (string.IsNullOrEmpty(soundDto.RecordingB64))
            {
                throw new NullReferenceException("Sound data is empty");
            }

            _fileService.AddEntrySound(soundDto.FileName, soundDto.RecordingB64!);

            var sound = SqlSound.FromDto(soundDto, _dbContext);
            var user = UserModel.FromEntity(_dbContext.Users.Find(_userId));
            sound.Rate = user.GetRateRange().Lower;
            _dbContext.Sounds.Add(sound);

            var changeSet = CreateChangeSetEntity(Operation.Insert, soundDto.SoundId);
            _dbContext.ChangeSets.Add(changeSet);
            await _dbContext.SaveChangesAsync();

            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<List<ChangeSetModel>> Update(PronunciationDto dto)
        {
            // Find out what has been changed
            var existing = await GetAsync(dto.SoundId);
            var existingDto = PronunciationDto.FromModel(existing);

            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                return new List<ChangeSetModel>();
            }

            // This should be before checking for privileges, in case when input is existing entry and nothing has been changed
            var user = UserModel.FromEntity(_dbContext.Users.Find(_userId));
            if (!user.CanEdit(existing.Rate, existing.UserId))
            {
                throw new InvalidOperationException("Error:Insufficient_privileges");
            }

            // Set rate, save
            ApplyChanges<SqlSound>(dto.SoundId, changes);

            var changeSet = CreateChangeSetEntity(Operation.Update, dto.SoundId);
            _dbContext.ChangeSets.Add(changeSet);
            await _dbContext.SaveChangesAsync();

            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<List<ChangeSetModel>> Remove(string entityId)
        {
            var sound = await _dbContext.Sounds.FindAsync(entityId);
            if (sound == null)
            {
                return new List<ChangeSetModel>();
            }

            var user = UserModel.FromEntity(await _dbContext.Users.FindAsync(_userId));
            if (!user.CanRemove(sound.Rate, sound.UserId, sound.CreatedAt))
            {
                throw new InvalidOperationException();
            }

            _fileService.DeleteEntrySound(sound.FileName);

            return await base.Remove(entityId);
        }

        public async Task<ChangeSetModel> Promote(ISound soundInfo)
        {
            var soundEntity = await _dbContext.Sounds.FindAsync(soundInfo.SoundId);
            if (soundEntity == null)
            {
                throw new NullReferenceException();
            }

            var userEntity = await _dbContext.Users.FindAsync(_userId);
            var user = UserModel.FromEntity(userEntity);

            var newRate = user.GetRateRange().Lower;

            soundEntity.Rate = newRate;
            await _dbContext.SaveChangesAsync();

            var changes = new List<Change>() { new Change() { OldValue = soundEntity.Rate, NewValue = newRate, Property = "Rate" } };
            var changeSet = CreateChangeSetEntity(Operation.Update, soundEntity.SoundId, changes);
            _dbContext.ChangeSets.Add(changeSet);
            await _dbContext.SaveChangesAsync();

            return ChangeSetModel.FromEntity(changeSet);
        }
    }
}
