using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_utils;
using chldr_utils.Interfaces;
using chldr_utils.Services;
using Realms;
using System.Threading.Channels;

namespace chldr_data.local.Repositories
{
    internal class RealmSoundsRepository : RealmRepository<RealmSound, SoundModel, SoundDto>, ISoundsRepository
    {
        public RealmSoundsRepository(Realm context, ExceptionHandler exceptionHandler, FileService fileService, string userId) : base(context, exceptionHandler, fileService, userId) { }
        protected override RecordType RecordType => RecordType.Sound;
        protected override SoundModel FromEntityShortcut(RealmSound entity)
        {
            return SoundModel.FromEntity(entity);
        }
        public override void Add(SoundDto dto)
        {
            var entry = _dbContext.Find<RealmEntry>(dto.EntryId);

            _dbContext.Write(() =>
            {
                var sound = RealmSound.FromDto(dto, _dbContext, entry);
                _dbContext.Add(sound);
            });

            _fileService.AddEntrySound(dto.FileName, dto.RecordingB64!);

            InsertChangeSet(Operation.Insert, dto.UserId!, dto.SoundId);
        }
        public override void Update(SoundDto dto)
        {
            var existingEntity = Get(dto.SoundId);
            var existingDto = SoundDto.FromModel(existingEntity);

            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                return;
            }

            _dbContext.Write(() =>
            {
                RealmSound.FromDto(dto, _dbContext);
            });

            InsertChangeSet(Operation.Update, dto.UserId!, dto.SoundId, changes);
        }
        public override void Remove(string entityId)
        {
            var sound = Get(entityId);
            _fileService.DeleteEntrySound(sound.FileName);

            base.Remove(entityId);
        }
    }
}