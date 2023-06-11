using chldr_data.Services;
using chldr_utils.Services;
using chldr_utils;
using chldr_data.local.Services;
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

            var connectionString = "server=104.248.40.142;port=3306;database=xj_chldr;user=;password=";
            var remoteSqlContext = new SqlDataProvider(connectionString);
            remoteSqlContext.Initialize();
            var sqlDatabase = remoteSqlContext.GetContext();

            var entries = realmDatabase.All<RealmEntry>();
            int count = 0;
            Console.WriteLine("Starting the update ...");

            foreach (var entry in entries)
            {
                var sqlEntry = sqlDatabase.Entries.FirstOrDefault(e => e.EntryId.Equals(entry.EntryId));
                if (sqlEntry == null)
                {
                    Console.Write("failed updating: ");
                    Console.WriteLine(entry.EntryId);
                    continue;
                }

                sqlEntry.Content = entry.Content;
                sqlEntry.RawContents = entry.RawContents;
                sqlEntry.Subtype = entry.Subtype;
                if (count % 100 == 0 || count > entries.Count() - 50)
                {
                    Console.WriteLine($"Saving {count}");
                    sqlDatabase.SaveChanges();
                }
                count++;
            }
            Console.WriteLine("Saving ...");
            Console.WriteLine($"Successfully saved {count} entries");
        }
    }
}