using Activity = Android.App.Activity;
using AndroidX.RecyclerView.Widget;
using chldr_android.Services;
using core.Interfaces;
using core.Models;
using chldr_domain.Interfaces;
using chldr_domain.Services;
using core.Services;
using chldr_utils.Services;
using chldr_app.Stores;
using core.DatabaseObjects.Models;
using chldr_app.Services;
using System.Diagnostics;

namespace chldr_android
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private static bool _isInitialized = false;
        private static readonly ServiceLocator _serviceLocator = new ServiceLocator();

        private ContentStore _contentStore;
        private EntriesAdapter _adapter;
        private EditText _txtSearch;
        private RecyclerView _rvEntries;

        private async Task Initialize()
        {
            // Make sure to run the following code only once
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

            // First time offline database deployment
            await AssetsService.ExtractInitialDataFromAssets(this, "data.zip");

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
    }
}