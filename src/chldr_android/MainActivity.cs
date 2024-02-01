using chldr_data.realm.Services;
using chldr_data.Services;
using chldr_utils;
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
                var fileService = new FileService(Application.Context.FilesDir!.AbsolutePath);
                var exceptionHandler = new ExceptionHandler(fileService);
                var environmentService = new EnvironmentService(chldr_data.Enums.Platforms.Android, true);

                var localStorageService = new JsonFileSettingsService(fileService, exceptionHandler);
                var graphQl = new GraphQLClient(exceptionHandler, environmentService, localStorageService);

                var requestService = new RequestService(graphQl);

                var syncService = new SyncService(requestService, fileService);
                _dataProvider = new RealmDataProvider(fileService, exceptionHandler, syncService);

                _dataProvider.DatabaseInitialized += DataProvider_DatabaseInitialized;

                ExtractFileFromAssets(this, "data.zip");
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