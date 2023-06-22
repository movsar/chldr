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

        private readonly FileService _fileService;
        string _connectionString;
        public SqlDataProvider(FileService fileService, string connectionString)
        {
            _fileService = fileService;
            _connectionString = connectionString;
        }

        //private string KeyAsString()
        //{
        //byte[] encryptionKey = File.ReadAllBytes(Path.Combine(_fileService.AppDataDirectory, "encryption.key"));

        //var key = encryptionKey.Select(b => (int)b);
        //var stringified = string.Join(":", key);
        //return stringified;
        //}

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

        private const string _defaultUserId = "63a816205d1af0e432fba6dd";

        public IUnitOfWork CreateUnitOfWork(string? userId = _defaultUserId)
        {
            var context = GetContext();
            return new SqlUnitOfWork(context, _fileService, userId!);
        }
    }
}