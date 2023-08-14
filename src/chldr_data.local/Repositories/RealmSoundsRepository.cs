using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_utils;
using chldr_utils.Services;

namespace chldr_data.local.Repositories
{
    public class RealmSoundsRepository : RealmRepository<RealmSound, PronunciationModel, PronunciationDto>, IPronunciationsRepository
    {
        public RealmSoundsRepository(ExceptionHandler exceptionHandler, FileService fileService, string userId) : base(exceptionHandler, fileService, userId) { }
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
    }
}