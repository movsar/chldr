using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_data.Repositories;
using chldr_utils;
using chldr_utils.Interfaces;
using chldr_utils.Services;
using Realms;

namespace chldr_data.local.Repositories
{
    internal class RealmSoundsRepository : RealmRepository<RealmSound, SoundModel, SoundDto>, ISoundsRepository
    {
        public RealmSoundsRepository(Realm context, ExceptionHandler exceptionHandler) : base(context, exceptionHandler) { }
        protected override RecordType RecordType => throw new NotImplementedException();
        protected override SoundModel FromEntityShortcut(RealmSound entity)
        {
            return SoundModel.FromEntity(entity);
        }
        public override void Add(SoundDto soundDto)
        {
            var entry = _dbContext.Find<RealmEntry>(soundDto.EntryId);

            _dbContext.Write(() =>
            {
                var sound = RealmSound.FromDto(soundDto, _dbContext, entry);
                _dbContext.Add(sound);
            });

            FileService.AddEntrySound(soundDto.FileName, soundDto.RecordingB64!);            
        }
        public override void Update(SoundDto soundDto)
        {
            var entry = _dbContext.Find<RealmEntry>(soundDto.EntryId);
            _dbContext.Write(() =>
            {
                RealmSound.FromDto(soundDto, _dbContext, entry);
            });
        }
        public override void Remove(string entityId)
        {
            var sound = Get(entityId);
            base.Remove(entityId);

            FileService.DeleteEntrySound(sound.FileName);
        }
    }
}