using chldr_data.Services;
using chldr_utils.Services;
using chldr_utils;
using chldr_tools.Services;
using chldr_data.Interfaces;
using chldr_data.local.Services;
using Microsoft.Extensions.Configuration;
using chldr_data.Repositories;
using chldr_data.local.RealmEntities;

namespace chldr_tools
{
    internal class Program
    {
        private static FileService _fileService;
        private static ExceptionHandler _exceptionHandler;
        private static NetworkService _networkService;
        private static EnvironmentService _environmentService;
        private static GraphQLRequestSender _requestSender;
        private static SyncService _syncService;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            _fileService = new FileService(AppContext.BaseDirectory);
            _exceptionHandler = new ExceptionHandler(_fileService);
            _networkService = new NetworkService();
            _environmentService = new EnvironmentService(chldr_shared.Enums.Platforms.Windows);
            _requestSender = new GraphQLRequestSender(_exceptionHandler);
            _syncService = new SyncService(_requestSender);

            var serviceLocator = new ServiceLocator();

            var localRealmContext = new RealmDataProvider(_fileService, _exceptionHandler, _requestSender, _syncService);
            localRealmContext.Initialize();
            var realmDatabase = localRealmContext.GetContext();

            var connectionString = "server=104.248.40.142;port=3306;database=xj_chldr;user=xj_admin;password=2398ehh&*H!&*Hhs-=";
            var remoteSqlContext = new SqlDataProvider(connectionString);
            remoteSqlContext.Initialize();
            var sqlDatabase = remoteSqlContext.GetContext();

            Thread.Sleep(1000);

            var entries = realmDatabase.All<RealmEntry>();
            foreach (var entry in entries)
            {
                var sqlEntry = sqlDatabase.Entries.Find(entry.EntryId);
                sqlEntry.Content = entry.Content;
                sqlEntry.RawContents = entry.RawContents;
                sqlEntry.Subtype = entry.Subtype;

            }

            //    var databaseOperations = new DatabaseOperations();
            //    databaseOperations.CopySqlToRealm();
        }
    }
}