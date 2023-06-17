using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;

namespace chldr_data.remote.Repositories
{
    internal class SqlSoundsRepository : SqlRepository<SqlSound, SoundModel, SoundDto>, ISoundsRepository
    {
        public SqlSoundsRepository(SqlContext context, string userId) : base(context, userId) { }

        protected override RecordType RecordType => RecordType.Sound;

        public override void Add(SoundDto dto)
        {
            throw new NotImplementedException();
        }

        public override void Update(SoundDto dto)
        {
            throw new NotImplementedException();
        }

        protected override SoundModel FromEntityShortcut(SqlSound entity)
        {
            throw new NotImplementedException();
        }
    }
}
