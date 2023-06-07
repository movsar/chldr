using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.SqlEntities;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.local.RealmEntities;
using chldr_tools;
using Realms;

namespace chldr_data.Repositories
{
    public class RealmSourcesRepository : RealmRepository<RealmSource, SourceModel, SourceDto>
    {
        protected override RecordType RecordType => RecordType.Source;
        public RealmSourcesRepository(Realm context) : base(context) { }

        public override IEnumerable<ChangeSetModel> Add(string userId, SourceDto dto)
        {
            throw new NotImplementedException();
        }
        public List<RealmSource> GetUnverifiedSources()
        {
            var sources = DbContext.All<RealmSource>().Where(s => s.Notes == "Imported from legacy database" || s.Name == "User");
            return sources.ToList();
        }

        public List<SourceModel> GetAllNamedSources()
        {
            return DbContext.All<RealmSource>().AsEnumerable().Select(s => SourceModel.FromEntity(s)).ToList();
        }

        public override SourceModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ChangeSetModel> Update(string userId, SourceDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
