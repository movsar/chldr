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
using Microsoft.Extensions.Configuration;

namespace chldr_data.local.Services
{
    public class SqlDataProvider : IDataProvider
    {
        public bool IsInitialized { get; set; }

        public event Action? DatabaseInitialized;
        string _connectionString;
        public SqlDataProvider(
            string connectionString
            )
        {
            _connectionString = connectionString;
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
        public SqlContext GetContext()
        {
            var options = new DbContextOptionsBuilder<SqlContext>()
                               .UseMySQL(_connectionString)
                               .Options;

            return new SqlContext(options);
        }

        public IUnitOfWork CreateUnitOfWork(string? userId = null)
        {
            if (userId == null)
            {
                throw new Exception("The active user must be set");
            }

            var context = GetContext();
            return new SqlUnitOfWork(context, userId);
        }
    }
}