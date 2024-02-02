using Activity = Android.App.Activity;
using AndroidX.RecyclerView.Widget;
using chldr_android.Services;
using chldr_app.Stores;
using core.DatabaseObjects.Models;
using core.Models;
using System.Diagnostics;

namespace chldr_android
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        // Making these static to persist across activity re-creation.
        private static ContentStore _contentStore;
        private static EntriesAdapter _adapter;

        // No need for these to be static, they're re-assigned on each onCreate.
        private EditText _txtSearch;
        private RecyclerView _rvEntries;
        private ImageButton _btnRefresh;
        private static bool _isInitialized = false;
        private static readonly ServiceLocator _serviceLocator = new ServiceLocator();

        private async Task Initialize()
        {
            if (_isInitialized)
            {
                return;
            }
            _isInitialized = true;

            if (ApplicationContext == null)
            {
                throw new Exception("Application context was null");
            }

            _serviceLocator.RegisterServices(ApplicationContext);
            await AssetsService.ExtractInitialDataFromAssets(this, "data.zip");

            _contentStore = _serviceLocator.GetService<ContentStore>();
            _contentStore.SearchResultsReady += OnNewSearchResults;
            _contentStore.ContentInitialized += OnContentInitialized;
            _contentStore.Initialize();
        }

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _txtSearch = FindViewById<EditText>(Resource.Id.txtSearchPhrase)!;
            _rvEntries = FindViewById<RecyclerView>(Resource.Id.rvMain)!;
            _btnRefresh = FindViewById<ImageButton>(Resource.Id.btnRefresh)!;

            if (_adapter == null)
            {
                _adapter = new EntriesAdapter(new List<EntryModel>());
            }
            _rvEntries.SetAdapter(_adapter);
            _rvEntries.SetLayoutManager(new LinearLayoutManager(this));

            _txtSearch.TextChanged += TxtSearch_TextChanged;
            _btnRefresh.Click += _btnRefresh_Click;

            if (!IsChangingConfigurations)
            {
                // Only run initialization if we're not in the middle of a configuration change.
                _ = Initialize().ConfigureAwait(false);
            }
        }

        private void _btnRefresh_Click(object? sender, EventArgs e)
        {
            _btnRefresh.Enabled = false;
            _contentStore.RequestRandomEntries();
        }

        private void OnContentInitialized()
        {
            _contentStore.RequestRandomEntries();
        }

        private void OnNewSearchResults(List<EntryModel> entries)
        {
            _btnRefresh.Enabled = true;
            RunOnUiThread(() =>
            {
                _adapter.UpdateEntries(entries);
            });
        }

        private async void TxtSearch_TextChanged(object? sender, Android.Text.TextChangedEventArgs e)
        {
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

            try
            {
                var searchTerm = e.Text?.ToString();
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return;
                }

                _contentStore.FindEntryDeferred(searchTerm, filtrationFlags);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AAAA! OnTextChanged");
            }
        }

        // Override to save the state of the application
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }

        // Override to restore state
        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
        }
    }
}
