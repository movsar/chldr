using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.local.RealmEntities;
using chldr_tools;
using chldr_utils.Interfaces;
using chldr_utils;
using Realms;

namespace chldr_data.Repositories
{
    public class RealmSourcesRepository : RealmRepository<RealmSource, SourceModel, SourceDto>, ISourcesRepository
    {
        protected override RecordType RecordType => RecordType.Source;
        public RealmSourcesRepository(Realm context, ExceptionHandler exceptionHandler, IGraphQLRequestSender graphQLRequestSender) : base(context, exceptionHandler, graphQLRequestSender) { }

        public override void Insert(SourceDto dto)
        {
            throw new NotImplementedException();
        }
        public List<RealmSource> GetUnverifiedSources()
        {
            var sources = _dbContext.All<RealmSource>().Where(s => s.Notes == "Imported from legacy database" || s.Name == "User");
            return sources.ToList();
        }

        public List<SourceModel> GetAllNamedSources()
        {
            return _dbContext.All<RealmSource>().AsEnumerable().Select(s => SourceModel.FromEntity(s)).ToList();
        }

        public override SourceModel Get(string entityId)
        {
            throw new NotImplementedException();
        }

        public override void Update(SourceDto dto)
        {
            throw new NotImplementedException();
        }

        public override void Delete(string entityId)
        {
            throw new NotImplementedException();
        }
    }
}
