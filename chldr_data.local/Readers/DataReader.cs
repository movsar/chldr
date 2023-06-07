using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_data.local.RealmEntities;
using chldr_data.Models;
using chldr_data.Services;
using Realms;

namespace chldr_data.Readers
{
    public class DataReader<TEntity, TModel> where TEntity : IEntity where TModel : class
    {
        protected Realm Database => Realm.GetInstance(IDataProvider.OfflineDatabaseConfiguration);
        
        
        //public TEntryModel GetById(string entityId)
        //{
        //    var entry = Database.All<RealmEntry>().FirstOrDefault(e => e.EntryId == entryId);
        //    if (entry == null)
        //    {
        //        throw new NullReferenceException();
        //    }

        //    var entryModel = EntryModelFactory.CreateEntryModel(entry) as TEntryModel;
        //    return entryModel!;
        //}

        //public List<TEntryModel> Take(int limit, int skip = 0)
        //{
        //    var entries = Database.All<RealmEntry>().AsEnumerable()
        //        .Skip(skip).Take(limit)
        //        .Select(e => EntryModelFactory.CreateEntryModel(e) as TEntryModel)
        //        .ToList();
        //    return entries;
        //}

    }
}
