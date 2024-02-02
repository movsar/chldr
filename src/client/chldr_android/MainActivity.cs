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
using chldr_app.Stores;
using core.DatabaseObjects.Models;
using chldr_app.Services;
using static Android.Security.Identity.CredentialDataResult;
using System.Diagnostics;

namespace chldr_android
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private static bool _isInitialized = false;
        private static ServiceLocator _serviceLocator;

        private ContentStore _contentStore;
        private EntriesAdapter _adapter;
        private EditText _txtSearch;
        private RecyclerView _rvEntries;
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
            // Make sure to run the following code only once
            if (_isInitialized)
            {
                return;
            }
            _isInitialized = true;

            #region Configure services
            _serviceLocator = new ServiceLocator();
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

            var dataProvider = new RealmDataProvider(fileService, exceptionHandler, syncService);
            _serviceLocator.RegisterService<IDataProvider>(dataProvider);

            var entryService = new EntryService(dataProvider, requestService, exceptionHandler, environmentService);
            _serviceLocator.RegisterService<EntryService>(entryService);

            var sourceService = new SourceService(dataProvider, requestService, exceptionHandler);
            _serviceLocator.RegisterService<SourceService>(sourceService);

            var settingsService = new AndroidSettingsService(this.ApplicationContext);
            _serviceLocator.RegisterService<ISettingsService>(settingsService);

            var userService = new UserService(dataProvider, requestService, settingsService);
            _serviceLocator.RegisterService<UserService>(userService);

            var entryCacheService = new EntryCacheService();
            _serviceLocator.RegisterService<EntryCacheService>(entryCacheService);

            var contentStore = new ContentStore(exceptionHandler, dataProvider, environmentService, sourceService, entryService, userService, entryCacheService);
            _serviceLocator.RegisterService<ContentStore>(contentStore);
            #endregion

            // First time offline database deployment
            await ExtractFileFromAssets(this, "data.zip");

            // Start
            _contentStore = _serviceLocator.GetService<ContentStore>();
            _contentStore.SearchResultsReady += OnNewSearchResults;
            _contentStore.ContentInitialized += OnContentInitialized;
            _contentStore.Initialize();
        }
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            try
            {

                base.OnCreate(savedInstanceState);
                _ = Initialize().ConfigureAwait(false);

                SetContentView(Resource.Layout.activity_main);

                _txtSearch = FindViewById<EditText>(Resource.Id.txtSearchPhrase)!;
                _rvEntries = FindViewById<RecyclerView>(Resource.Id.rvMain)!;
                if (_adapter == null)
                {
                    _adapter = new EntriesAdapter(new List<EntryModel>());
                    _rvEntries.SetAdapter(_adapter);
                    _rvEntries.SetLayoutManager(new LinearLayoutManager(this));
                }

                _txtSearch.TextChanged += TxtSearch_TextChanged;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AAAA! Oncreate");
            }
        }
        private void OnContentInitialized()
        {
            try
            {
                _contentStore.RequestRandomEntries();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AAAA! OnContentIntialized");
            }
        }
        private void OnNewSearchResults(List<EntryModel> entries)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    _adapter.UpdateEntries(entries);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("AAAA! OnNewSearchResults");
                }
            });
        }

        private async void TxtSearch_TextChanged(object? sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {

                var searchTerm = e.Text?.ToString();
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return;
                }

                _contentStore.FindEntryDeferred(searchTerm);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AAAA! OnTextChanged");
            }
        }
    }
}