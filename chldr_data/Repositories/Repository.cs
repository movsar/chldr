using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Interfaces.DatabaseEntities;
using chldr_data.Models;
using chldr_data.Models.Words;
using Newtonsoft.Json;
using Realms;
using System.Runtime.CompilerServices;

namespace chldr_data.Repositories
{
    public abstract class Repository
    {
        public IDataAccess DataAccess { get; }
        public Realm Database => DataAccess.GetActiveDataservice().GetDatabase();
        public Repository(IDataAccess dataAccess)
        {
            DataAccess = dataAccess;
        }
        protected async Task Sync(List<IChangeSet>? changeSets = null)
        {
            var changeSetsToApply = changeSets;
            if (changeSetsToApply == null)
            {
                // TODO: Get latest changesets based on...?
            }

            foreach (var changeSet in changeSetsToApply)
            {
                if (changeSet.RecordType == Enums.RecordType.word)
                {
                    var updatedEntry = JsonConvert.DeserializeObject<WordModel>(changeSet.RecordValue);
                }
            }

        }
    }
}