using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.DatabaseObjects.Models;
using chldr_utils;
using chldr_utils.Services;
using chldr_utils.Interfaces;
using chldr_data.remote.Services;
using chldr_data.Services;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.local.Services
{
    public class SqlDataProvider : IDataProvider
    {
        private readonly ExceptionHandler _exceptionHandler;

        public bool IsInitialized { get; set; }

        public event Action? DatabaseInitialized;
        public event Action<EntryModel>? EntryUpdated;
        public event Action<EntryModel>? EntryInserted;
        public event Action<EntryModel>? EntryDeleted;
        public event Action<EntryModel>? EntryAdded;

        public SqlDataProvider(
            ExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
        }

        private string KeyAsString()
        {
            byte[] encryptionKey = File.ReadAllBytes(Path.Combine(FileService.AppDataDirectory, "encryption.key"));

            var key = encryptionKey.Select(b => (int)b);
            var stringified = string.Join(":", key);
            return stringified;
        }

        public void Initialize()
        {
            DatabaseInitialized?.Invoke();
        }

        public void PurgeAllData()
        {
            //_sqlContext.Entries.Remove();
            //database.Write(() =>
            //{
            //    database.RemoveAll<RealmEntry>();
            //    database.RemoveAll<RealmText>();
            //    database.RemoveAll<RealmWord>();
            //    database.RemoveAll<RealmPhrase>();
            //    database.RemoveAll<RealmTranslation>();
            //});
        }
        public SqlContext GetDatabaseContext()
        {
            var connectionString = "server=104.248.40.142;port=3306;database=xj_chldr;user=xj_admin;password=2398ehh&*H!&*Hhs-=";
            var options = new DbContextOptionsBuilder<SqlContext>()
                               .UseMySQL(connectionString)
                               .Options;

            return new SqlContext(options);
        }
        public IUnitOfWork CreateUnitOfWork()
        {
            var context = GetDatabaseContext();
            return new SqlUnitOfWork(context);
        }
    }
}