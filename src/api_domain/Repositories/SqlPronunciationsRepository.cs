using core.DatabaseObjects.Dtos;
using core.DatabaseObjects.Interfaces;
using core.DatabaseObjects.Models;
using core.Enums;
using core.Interfaces;
using core.Interfaces.Repositories;
using core.Models;
using api_domain.Services;
using api_domain.SqlEntities;
using chldr_utils.Services;
using Microsoft.EntityFrameworkCore;
using core;

namespace api_domain.Repositories
{
    public class SqlPronunciationsRepository : SqlRepository<SqlSound, PronunciationModel, PronunciationDto>, IPronunciationsRepository
    {
        public SqlPronunciationsRepository(SqlContext context, IFileService fileService, string userId) : base(context, fileService, userId) { }
        protected override RecordType RecordType => RecordType.Sound;
        public override async Task<List<PronunciationModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();
            var ids = await _dbContext.Set<SqlSound>().Select(e => e.SoundId).ToListAsync();
            var randomlySelectedIds = ids.OrderBy(x => randomizer.Next(1, Constants.EntriesApproximateCoount)).Take(limit).ToList();

            var entities = await _dbContext.Set<SqlSound>()
              .Where(e => randomlySelectedIds.Contains(e.SoundId))
              .AsNoTracking()
              .ToListAsync();

            var models = entities.Select(PronunciationModel.FromEntity).ToList();
            return models;
        }
        public override async Task<List<ChangeSetModel>> AddAsync(PronunciationDto soundDto)
        {
            if (string.IsNullOrEmpty(soundDto.RecordingB64))
            {
                throw new NullReferenceException("Sound data is empty");
            }

            _fileService.AddEntrySound(soundDto.FileName, soundDto.RecordingB64!);

            var sound = SqlSound.FromDto(soundDto, _dbContext);
            var user = UserModel.FromEntity(_dbContext.Users.Find(_userId) as SqlUser);
            sound.Rate = user.GetRateRange().Lower;
            _dbContext.Pronunciations.Add(sound);

            var changeSet = CreateChangeSetEntity(Operation.Insert, soundDto.SoundId);
            _dbContext.ChangeSets.Add(changeSet);
            await _dbContext.SaveChangesAsync();

            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<List<ChangeSetModel>> UpdateAsync(PronunciationDto dto)
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
            var user = UserModel.FromEntity(_dbContext.Users.Find(_userId) as SqlUser);
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

        public override async Task<List<ChangeSetModel>> RemoveAsync(string entityId)
        {
            var sound = await _dbContext.Pronunciations.FindAsync(entityId);
            if (sound == null)
            {
                return new List<ChangeSetModel>();
            }

            var user = UserModel.FromEntity(await _dbContext.Users.FindAsync(_userId) as SqlUser);
            if (!user.CanRemove(sound.Rate, sound.UserId, sound.CreatedAt))
            {
                throw new InvalidOperationException();
            }

            _fileService.DeleteEntrySound(sound.FileName);

            return await base.RemoveAsync(entityId);
        }

        public async Task<ChangeSetModel> Promote(IPronunciation soundInfo)
        {
            var soundEntity = await _dbContext.Pronunciations.FindAsync(soundInfo.SoundId);
            if (soundEntity == null)
            {
                throw new NullReferenceException();
            }

            var userEntity = await _dbContext.Users.FindAsync(_userId);
            var user = UserModel.FromEntity(userEntity as SqlUser);

            var newRate = user.GetRateRange().Lower;

            soundEntity.Rate = newRate;
            await _dbContext.SaveChangesAsync();

            var changes = new List<Change>() { new Change() { OldValue = soundEntity.Rate, NewValue = newRate, Property = "Rate" } };
            var changeSet = CreateChangeSetEntity(Operation.Update, soundEntity.SoundId, changes);
            _dbContext.ChangeSets.Add(changeSet);
            await _dbContext.SaveChangesAsync();

            return ChangeSetModel.FromEntity(changeSet);
        }

        public override async Task<PronunciationModel> GetAsync(string entityId)
        {
            var pronunciation = await _dbContext.Pronunciations.FirstOrDefaultAsync(t => t.SoundId!.Equals(entityId));
            if (pronunciation == null)
            {
                throw new NullReferenceException();
            }

            return PronunciationModel.FromEntity(pronunciation);
        }

        public override async Task<List<PronunciationModel>> TakeAsync(int offset, int limit)
        {
            var entities = await _dbContext.Pronunciations
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return entities.Select(PronunciationModel.FromEntity).ToList();
        }

        public Task RemoveRange(string[] sounds)
        {
            throw new NotImplementedException();
        }
    }
}
