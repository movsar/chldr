using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.RealmEntities;
using MongoDB.Bson;

namespace chldr_data.Repositories
{
    public class SourcesRepository : Repository
    {
        public SourcesRepository(IDataAccess dataAccess) : base(dataAccess) { }

        public List<RealmSource> GetUnverifiedSources()
        {
            var sources = Database.All<RealmSource>().Where(s => s.Notes == "Imported from legacy database" || s.Name == "User");
            return sources.ToList();
        }

        public List<SourceModel> GetAllNamedSources()
        {
            return Database.All<RealmSource>().AsEnumerable().Select(s => SourceModel.FromEntity(s)).ToList();
        }

        public string Insert(SourceDto sourceDto)
        {
            if (!string.IsNullOrEmpty(sourceDto.SourceId))
            {
                throw new InvalidOperationException();
            }

            var source = new RealmSource()
            {
                Name = sourceDto.Name,
                Notes = sourceDto.Notes
            };

            Database.Write(() =>
            {
                Database.Add(source);
            });

            return source.SourceId;
        }
    }
}
