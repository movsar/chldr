using Data.Entities;
using Realms;
using System.Collections.Immutable;
using System.Diagnostics;
using MongoDB.Bson;
using Data.Models;
using Data.Search;
using Data.Enums;
using Data.Interfaces;
using Data.Services.PartialMethods;
using Realms.Sync;
namespace Data.Services
{
    public class OfflineDataAccess : DataAccess
    {
        Realm RealmDatabase = null;

        private static RealmConfiguration GetRealmConfiguration()
        {
            var dbPath = Path.Combine(FileService.AppDataDirectory, FileService.DatabaseName);

            return new RealmConfiguration(dbPath)
            {
                SchemaVersion = 14,
                ShouldCompactOnLaunch = (totalBytes, usedBytes) =>
                {
                    ulong oneHundredMB = 30 * 1024 * 1024;
                    return totalBytes > oneHundredMB && usedBytes / totalBytes < 0.5;
                }
            };
        }
        public OfflineDataAccess()
        {
            var fs = new FileService();
            fs.PrepareDatabase();

            RealmDatabase = Realm.GetInstance(GetRealmConfiguration());
        }
        internal override Realm GetRealmInstance()
        {
            return Realm.GetInstance(GetRealmConfiguration());
        }

    }
}
