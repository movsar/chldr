using chldr_data.Services;
using chldr_utils.Services;
using chldr_utils;
using chldr_tools.Services;
using chldr_data.Interfaces;

namespace chldr_tools
{
    internal class Program
    {
        private static FileService _fileService;
        private static ExceptionHandler _exceptionHandler;
        private static NetworkService _networkService;
        private static EnvironmentService _environmentService;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            _fileService = new FileService(AppContext.BaseDirectory);
            _exceptionHandler = new ExceptionHandler(_fileService);
            _networkService = new NetworkService();
            _environmentService = new EnvironmentService(chldr_shared.Enums.Platforms.Windows);

            var realmService = new RealmDataSource(_fileService, _exceptionHandler);
            realmService.InitializeDatabase();

        //    var databaseOperations = new DatabaseOperations();
        //    databaseOperations.CopySqlToRealm();
        }
    }
}