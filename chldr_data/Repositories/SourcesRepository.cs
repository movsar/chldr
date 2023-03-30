using chldr_data.Entities;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Repositories
{
    public class SourcesRepository : Repository
    {
        public SourcesRepository(IDataAccess dataAccess) : base(dataAccess) { }

        public List<Source> GetUnverifiedSources()
        {
            var sources = Database.All<Source>().Where(s => s.Notes == "Imported from legacy database" || s.Name == "User");
            return sources.ToList();
        }

        public List<SourceModel> GetAllNamedSources()
        {
            return Database.All<Source>().AsEnumerable().Select(s => new SourceModel(s)).ToList();
        }


    }
}
