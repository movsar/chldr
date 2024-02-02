using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces.Repositories;
using chldr_data.realm.RealmEntities;
using chldr_data.Models;
using chldr_data.Interfaces;

namespace chldr_data.Repositories
{
    public class RealmSourcesRepository : RealmRepository<RealmSource, SourceModel, SourceDto>, ISourcesRepository
    {
        public RealmSourcesRepository(IExceptionHandler exceptionHandler, IFileService fileService, string userId) : base(exceptionHandler, fileService, userId) { }
        protected override RecordType RecordType => RecordType.Source;
        public override SourceModel FromEntity(RealmSource entry)
        {
            return SourceModel.FromEntity(entry);
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
        public override async Task<List<ChangeSetModel>> Add(SourceDto dto)
        {
            _dbContext.Write(() =>
            {
                var entity = RealmSource.FromDto(dto, _dbContext);
                _dbContext.Add(entity);
            });
            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }
        public override async Task<List<ChangeSetModel>> Update(SourceDto dto)
        {
            var existingEntity = await GetAsync(dto.SourceId);
            var existingDto = SourceDto.FromModel(existingEntity);

            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                // ! NOT IMPLEMENTED
                return new List<ChangeSetModel>();
            }

            _dbContext.Write(() =>
            {
                var entity = RealmSource.FromDto(dto, _dbContext);
            });

            // ! NOT IMPLEMENTED
            return new List<ChangeSetModel>();
        }

        public override Task<List<ChangeSetModel>> Remove(string entityId)
        {
            throw new NotImplementedException();
        }
    }
}
