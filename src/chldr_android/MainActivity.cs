using AndroidX.RecyclerView.Widget;
using chldr_android.Services;
using core.Interfaces;
using core.Models;
using chldr_domain.Interfaces;
using chldr_domain.Services;
using core.Services;
using chldr_utils.Services;
using System.IO.Compression;
using Activity = Android.App.Activity;

namespace chldr_android
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private static bool _isInitialized = false;
        private static ServiceLocator _serviceLocator;

        private RealmDataProvider _dataProvider;
        private EntriesAdapter _adapter;

        public async Task ExtractFileFromAssets(Activity activity, string zipName)
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

                // Give it some time to unpack and rest :)
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while extracting the data file", ex);
            }
        }
        private async Task Initialize()
        {
            if (!_isInitialized)
            {
                _serviceLocator = new ServiceLocator();

                #region Register services
                var fileService = new FileService(Application.Context.FilesDir!.AbsolutePath);
                _serviceLocator.RegisterService<IFileService>(fileService);

                var exceptionHandler = new ExceptionHandler(_serviceLocator.GetService<IFileService>());
                _serviceLocator.RegisterService<IExceptionHandler>(exceptionHandler);

                var environmentService = new EnvironmentService(core.Enums.Platforms.Android, true);
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
                #endregion

                await ExtractFileFromAssets(this, "data.zip");
                _isInitialized = true;

                _dataProvider.DatabaseInitialized += DataProvider_DatabaseInitialized;
                _dataProvider.Initialize();
            }
        }
        protected override async void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            await Initialize();

            EditText txtSearch = FindViewById<EditText>(Resource.Id.txtSearchPhrase)!;
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        private async void TxtSearch_TextChanged(object? sender, Android.Text.TextChangedEventArgs e)
        {
            var searchTerm = e.Text?.ToString();
            if (string.IsNullOrEmpty(searchTerm))
            {
                return;
            }

            var filtrationFlags = new FiltrationFlags()
            {
                EntryFilters = new EntryFilters()
                {
                    IncludeOnModeration = false,
                },
                TranslationFilters = new TranslationFilters()
                {
                    IncludeOnModeration = false
                }
            };

            var repositories = _dataProvider.Repositories(null);
            var foundEntries = await repositories.Entries.FindAsync(searchTerm, filtrationFlags);

            _adapter.UpdateEntries(foundEntries);
        }

        private async void DataProvider_DatabaseInitialized()
        {
            var repositories = _dataProvider.Repositories(null);
            var foundEntries = await repositories.Entries.FindAsync("привет");

            var rvEntries = FindViewById<RecyclerView>(Resource.Id.rvMain)!;

            _adapter = new EntriesAdapter(foundEntries);
            rvEntries.SetAdapter(_adapter);

            // Optionally, if your RecyclerView doesn't set its LayoutManager in XML
            rvEntries.SetLayoutManager(new LinearLayoutManager(this));
        }
    }
}