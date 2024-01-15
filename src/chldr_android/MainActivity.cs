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
        private RealmDataProvider _dataProvider;

        public async Task ExtractDataFiles(Activity activity)
        {
            string appDataPath = Application.Context.FilesDir.AbsolutePath + Path.PathSeparator + "data";
            Directory.CreateDirectory(appDataPath);
            if (File.Exists($"{appDataPath}/database/local.datx"))
            {
                return;
            }

            try
            {
                // Get the ZIP file as a stream from the raw resources
                using (Stream zipStream = activity.Assets.Open("data.zip"))
                {
                    // Use ZipArchive to extract the ZIP file
                    using (ZipArchive archive = new ZipArchive(zipStream))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            string fullPath = Path.Combine(appDataPath, entry.FullName);
                            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                            // Extract the file asynchronously
                            using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                            {
                                await entry.Open().CopyToAsync(fileStream);
                            }
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

            var fileService = new FileService(Application.Context.FilesDir!.AbsolutePath);
            var exceptionHandler = new ExceptionHandler(fileService);
            var environmentService = new EnvironmentService(chldr_data.Enums.Platforms.Android, true);

            var localStorageService = new JsonFileSettingsService(fileService, exceptionHandler);
            var graphQl = new GraphQLClient(exceptionHandler, environmentService, localStorageService);

            var requestService = new RequestService(graphQl);

            var syncService = new SyncService(requestService, fileService);
            _dataProvider = new RealmDataProvider(fileService, exceptionHandler, syncService);

            _dataProvider.DatabaseInitialized += DataProvider_DatabaseInitialized;
            _dataProvider.Initialize();
        }

        private async void DataProvider_DatabaseInitialized()
        {
            var repositories = _dataProvider.Repositories(null);
            var a = await repositories.Entries.FindAsync("привет");
            Debug.WriteLine($"a[0].Content = {a[0].Content}");
        }

        protected override async void OnResume()
        {
            base.OnResume();
            await ExtractDataFiles(this);
        }
    }
}