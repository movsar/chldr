using chldr_data.Entities;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Realms.Sync.MongoClient;

namespace chldr_data.Repositories
{
    public abstract class EntriesRepository<TEntryModel> : Repository where TEntryModel : EntryModel
    {
        public EntriesRepository(IRealmService realmService) : base(realmService) { }

        public TEntryModel GetByEntryId(ObjectId entryId)
        {
            var entry = Database.All<Entry>().FirstOrDefault(e => e._id == entryId);
            if (entry == null)
            {
                throw new NullReferenceException();
            }

            var entryModel = EntryModelFactory.CreateEntryModel(entry) as TEntryModel;
            return entryModel!;
        }
    }
}
