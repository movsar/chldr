using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_data.Models;
using MongoDB.Bson;

namespace chldr_data.Repositories
{
    public class SourcesRepository : Repository
    {
        public SourcesRepository(IDataAccess dataAccess) : base(dataAccess) { }

        public List<SqlSource> GetUnverifiedSources()
        {
            var sources = Database.All<SqlSource>().Where(s => s.Notes == "Imported from legacy database" || s.Name == "User");
            return sources.ToList();
        }

        public List<SourceModel> GetAllNamedSources()
        {
            return Database.All<SqlSource>().AsEnumerable().Select(s => new SourceModel(s)).ToList();
        }

        public string Insert(SourceDto sourceDto)
        {
            if (!string.IsNullOrEmpty(sourceDto.SourceId))
            {
                throw new InvalidOperationException();
            }

            var source = new SqlSource()
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
