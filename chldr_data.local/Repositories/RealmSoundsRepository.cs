using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_data.Repositories;
using chldr_utils;
using chldr_utils.Interfaces;
using Realms;

namespace chldr_data.local.Repositories
{
    internal class RealmSoundsRepository : RealmRepository<RealmSound, SoundModel, SoundDto>, ISoundsRepository
    {
        public RealmSoundsRepository(Realm context, ExceptionHandler exceptionHandler) : base(context, exceptionHandler)
        {
        }

        protected override RecordType RecordType => throw new NotImplementedException();

        public override void Add(SoundDto dto)
        {
            throw new NotImplementedException();
        }

        public override void Update(SoundDto EntryDto)
        {
            throw new NotImplementedException();
        }

        protected override SoundModel FromEntityShortcut(RealmSound entity)
        {
            throw new NotImplementedException();
        }
    }
}
