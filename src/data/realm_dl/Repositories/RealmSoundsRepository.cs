using domain.DatabaseObjects.Dtos;
using domain.DatabaseObjects.Interfaces;
using domain.DatabaseObjects.Models;
using domain;
using domain.Interfaces.Repositories;
using realm_dl.RealmEntities;
using domain.Models;
using domain.Interfaces;
using domain.Enums;

namespace realm_dl.Repositories
{
    public class RealmSoundsRepository : RealmRepository<RealmSound, SoundModel, SoundDto>, ISoundsRepository
    {
        public RealmSoundsRepository(IExceptionHandler exceptionHandler, IFileService fileService, string userId) : base(exceptionHandler, fileService, userId) { }
        protected override RecordType RecordType => RecordType.Sound;
        public override SoundModel FromEntity(RealmSound entity)
        {
            return SoundModel.FromEntity(entity);
        }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task<List<ChangeSetModel>> Add(SoundDto dto)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var entry = _dbContext.Find<RealmEntry>(dto.EntryId);

            _dbContext.Write(() =>
            {
                var sound = RealmSound.FromDto(dto, _dbContext, entry);
                _dbContext.Add(sound);
            });

            _fileService.AddEntrySound(dto.FileName, dto.RecordingB64!);

            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }
        public override async Task<List<ChangeSetModel>> Update(SoundDto dto)
        {
            var existingEntity = await GetAsync(dto.SoundId);
            var existingDto = SoundDto.FromModel(existingEntity);

            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                // ! NOT IMPLEMENTED
                return new List<ChangeSetModel>();
            }

            _dbContext.Write(() =>
            {
                RealmSound.FromDto(dto, _dbContext);
            });

            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }
        public override async Task<List<ChangeSetModel>> Remove(string entityId)
        {
            // TODO: Remove remote entity

            var sound = await GetAsync(entityId);
            _fileService.DeleteEntrySound(sound.FileName);

            // Remove local entity
            var entity = _dbContext.Find<RealmSound>(entityId);
            if (entity == null)
            {
                return new List<ChangeSetModel>();
            }

            _dbContext.Write(() =>
            {
                _dbContext.Remove(entity);
            });

            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }

        public Task<ChangeSetModel> Promote(ISound sound)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(string[] sounds)
        {
            throw new NotImplementedException();
        }
    }
}