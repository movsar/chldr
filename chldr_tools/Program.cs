using chldr_data.Services;
using chldr_utils.Services;
using chldr_utils;
using chldr_data.local.Services;
using chldr_data.local.RealmEntities;
using Realms;
using chldr_data.remote.Services;

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

        static void ContentUpdater(Realm realmDatabase, SqlContext sqlDatabase)
        {

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

        static void LanguageCodeLocal(Realm realmDatabase)
        {
            var translations = realmDatabase.All<RealmTranslation>();
            int count = 0;
            Console.WriteLine("Starting the update ...");
            var total = translations.Count();
            realmDatabase.Write(() =>
            {

                foreach (var translation in translations)
                {
                    //translation.LanguageCode = translation.Language.Code;
                    if (count % 100 == 0 || count > total - 50)
                    {
                        Console.WriteLine($"Processed {count}");
                    }
                    count++;
                }
            });

            Console.WriteLine("Saving ...");
            Console.WriteLine($"Successfully saved {count} entries");

            realmDatabase.WriteCopy(new RealmConfiguration("new.realm"));
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            _fileService = new FileService(AppContext.BaseDirectory);
            _exceptionHandler = new ExceptionHandler(_fileService);
            _networkService = new NetworkService();
            _environmentService = new EnvironmentService(chldr_shared.Enums.Platforms.Windows, true);
            _requestSender = new GraphQLRequestSender(_exceptionHandler, _environmentService);
            _syncService = new SyncService(_requestSender);

            var serviceLocator = new ServiceLocator();

            var localRealmContext = new RealmDataProvider(_fileService, _exceptionHandler, _requestSender, _syncService);
            localRealmContext.Initialize();
            var realmDatabase = localRealmContext.GetContext();

            var connectionString = "asfsdfsadf";
            var remoteSqlContext = new SqlDataProvider(connectionString);
            remoteSqlContext.Initialize();
            var sqlDatabase = remoteSqlContext.GetContext();

            LanguageCodeLocal(realmDatabase);
        }
    }
}