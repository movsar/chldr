using core.DatabaseObjects.Dtos;
using core.DatabaseObjects.Interfaces;
using core.DatabaseObjects.Models;
using core.Enums;
using core.Interfaces.Repositories;
using chldr_domain.RealmEntities;
using core.Models;
using core.Repositories;
using core.Interfaces;

namespace chldr_domain.Repositories
{
    public class RealmSoundsRepository : RealmRepository<RealmSound, PronunciationModel, PronunciationDto>, IPronunciationsRepository
    {
        public RealmSoundsRepository(IExceptionHandler exceptionHandler, IFileService fileService, string userId) : base(exceptionHandler, fileService, userId) { }
        protected override RecordType RecordType => RecordType.Sound;
        public override PronunciationModel FromEntity(RealmSound entity)
        {
            return PronunciationModel.FromEntity(entity);
        }
        public override async Task<List<ChangeSetModel>> Add(PronunciationDto dto)
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
        public override async Task<List<ChangeSetModel>> Update(PronunciationDto dto)
        {
            var existingEntity = await GetAsync(dto.SoundId);
            var existingDto = PronunciationDto.FromModel(existingEntity);

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

        public Task<ChangeSetModel> Promote(IPronunciation sound)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(string[] sounds)
        {
            throw new NotImplementedException();
        }
    }
}