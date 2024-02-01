using chldr_android.Services;
using chldr_data.Interfaces;
using chldr_data.realm.Interfaces;
using chldr_data.realm.Services;
using chldr_data.Services;
using chldr_utils;
using chldr_utils.Interfaces;
using chldr_utils.Services;
using System.Diagnostics;
using System.IO.Compression;
using Activity = Android.App.Activity;

namespace chldr_android
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        static bool _isInitialized = false;
        private RealmDataProvider _dataProvider;
        private ServiceLocator _serviceLocator;

        public void ExtractFileFromAssets(Activity activity, string zipName)
        {
            if (Application.Context.FilesDir == null)
            {
                throw new Exception("Files dir is null");
            }

            string appDataPath = $"{Application.Context.FilesDir.AbsolutePath}/data";
            if (File.Exists($"{appDataPath}/database/local.datx"))
            {
                return;
            }

            if (activity.Assets == null)
            {
                throw new Exception("Assets was null");
            }

            try
            {
                // Get the ZIP file as a stream from the raw resources
                using (Stream zipStream = activity.Assets.Open(zipName))
                using (ZipArchive archive = new ZipArchive(zipStream))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string entryPath = Path.Combine(appDataPath, entry.FullName);
                        if (entry.FullName.EndsWith("/"))
                        {
                            Directory.CreateDirectory(entryPath);
                        }
                        else
                        {
                            // Ensure the file's directory exists
                            Directory.CreateDirectory(Path.GetDirectoryName(entryPath)!);

                            // Extract the file
                            entry.ExtractToFile(entryPath, overwrite: true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error extracting ZIP file: " + ex.Message);
            }
        }
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            if (!_isInitialized)
            {
                _serviceLocator = new ServiceLocator();

                // Register services
                var fileService = new FileService(Application.Context.FilesDir!.AbsolutePath);
                _serviceLocator.RegisterService<IFileService>(fileService);

                var exceptionHandler = new ExceptionHandler(_serviceLocator.GetService<IFileService>());
                _serviceLocator.RegisterService<IExceptionHandler>(exceptionHandler);

                var environmentService = new EnvironmentService(chldr_data.Enums.Platforms.Android, true);
                _serviceLocator.RegisterService<IEnvironmentService>(environmentService);

                var localStorageService = new JsonFileSettingsService(fileService, exceptionHandler);
                _serviceLocator.RegisterService<ISettingsService>(localStorageService);

                var graphQl = new GraphQLClient(exceptionHandler, environmentService, localStorageService);
                _serviceLocator.RegisterService<IGraphQlClient>(graphQl);

                var requestService = new RequestService(graphQl);
                _serviceLocator.RegisterService<IRequestService>(requestService);

                var syncService = new SyncService(requestService, fileService);
                _serviceLocator.RegisterService<ISyncService>(syncService);

                _dataProvider = new RealmDataProvider(fileService, exceptionHandler, syncService);
                _serviceLocator.RegisterService<IDataProvider>(_dataProvider);


                ExtractFileFromAssets(this, "data.zip");

                _dataProvider.DatabaseInitialized += DataProvider_DatabaseInitialized;
                _dataProvider.Initialize();
            }
        }

        private async void DataProvider_DatabaseInitialized()
        {
            var repositories = _dataProvider.Repositories(null);
            var a = await repositories.Entries.FindAsync("привет");
            Debug.WriteLine($"a[0].Content = {a[0].Content}");
        }
    }
}